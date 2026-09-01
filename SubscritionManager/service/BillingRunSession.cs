namespace SubscritionManager;

public sealed class BillingRunSession : IDisposable
{
    private readonly StreamWriter _invoiceLog;
    private readonly Action<string>? _resourceEventLogger;
    private bool _disposed;

    public BillingRunSession(
        Stream invoiceLogStream,
        Action<string>? resourceEventLogger = null)
    {
        _resourceEventLogger = resourceEventLogger;
        GatewayConnection = new PaymentGatewayConnection(resourceEventLogger);
        _invoiceLog = new StreamWriter(invoiceLogStream, leaveOpen: true);
    }

    public PaymentGatewayConnection GatewayConnection { get; }

    public void RenewSubscription(Subscription subscription, int gracePeriodDays)
    {
        ThrowIfDisposed();

        if (subscription.Status == Status.Cancelled)
            throw new NotSupportedException("Cancelled subscriptions cannot be renewed.");

        if (subscription.DaysPastDue > gracePeriodDays)
            throw new SubscriptionExpiredException(subscription.Id, subscription.DaysPastDue);

        GatewayConnection.ChargeRenewal(subscription);

        var invoiceEntry = $"Subscription={subscription.Id};Amount={subscription.RenewalAmount}";
        _invoiceLog.WriteLine(invoiceEntry);
    }

    public void RenewSubscription(Subscription subscription)
    {
        RenewSubscription(subscription, gracePeriodDays: 0);
    }

    public void ProcessRenewals(
        IEnumerable<Subscription> subscriptions,
        int gracePeriodDays,
        Action<Subscription, Exception>? failureHandler = null)
    {
        ThrowIfDisposed();

        foreach (var subscription in subscriptions)
        {
            try
            {
                RenewSubscription(subscription, gracePeriodDays);
            }
            catch (SubscriptionExpiredException exception)
            {
                failureHandler?.Invoke(subscription, exception);
            }
            catch (PaymentGatewayException exception)
            {
                failureHandler?.Invoke(subscription, exception);
            }
            catch (NotSupportedException exception)
            {
                failureHandler?.Invoke(subscription, exception);
            }
            finally
            {
                _resourceEventLogger?.Invoke($"SubscriptionProcessed:{subscription.Id}");
            }
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    ~BillingRunSession()
    {
        Dispose(disposing: false);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        GatewayConnection.Close();

        if (disposing)
        {
            _resourceEventLogger?.Invoke("InvoicesFlushed");
            _invoiceLog.Flush();
            _invoiceLog.Dispose();
        }

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(BillingRunSession));
    }
}
