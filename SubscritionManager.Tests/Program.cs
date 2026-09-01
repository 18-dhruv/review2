using System.Runtime.CompilerServices;
using System.Text;
using SubscritionManager;

RunDisposeReleasesResourcesAfterRenewalException();
RunFinalizerClosesGatewayWhenDisposeIsMissed();
RunBatchContinuesAndWritesOnlyCompleteInvoices();

Console.WriteLine("BillingRunSession tests passed.");

static void RunDisposeReleasesResourcesAfterRenewalException()
{
    var resourceEvents = new List<string>();
    using var invoiceStream = new MemoryStream();
    var session = new BillingRunSession(invoiceStream, resourceEvents.Add);
    var gatewayConnection = session.GatewayConnection;

    try
    {
        session.RenewSubscription(new Subscription { Id = 1, RenewalAmount = 99 });
        session.RenewSubscription(new Subscription { Id = 2, RenewalAmount = -1 });
        throw new Exception("Expected renewal to throw for a negative amount.");
    }
    catch (PaymentGatewayException)
    {
    }
    finally
    {
        session.Dispose();
    }

    Assert(gatewayConnection.IsClosed, "Gateway should be closed after Dispose.");
    Assert(
        resourceEvents.SequenceEqual(["GatewayClosed", "InvoicesFlushed"]),
        "Dispose should close the gateway before flushing invoices.");

    invoiceStream.Position = 0;
    var invoiceLog = Encoding.UTF8.GetString(invoiceStream.ToArray());

    Assert(
        invoiceLog.Contains("Subscription=1;Amount=99"),
        "Successful renewal should be written to invoice log.");
}

static void RunFinalizerClosesGatewayWhenDisposeIsMissed()
{
    var resourceEvents = new List<string>();
    var gatewayConnection = CreateUndisposedSessionGateway(resourceEvents.Add);

    for (var attempt = 0; attempt < 10 && !gatewayConnection.IsClosed; attempt++)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Thread.Sleep(50);
    }

    Assert(gatewayConnection.IsClosed, "Finalizer should close the gateway when Dispose is missed.");
    Assert(
        resourceEvents.SequenceEqual(["GatewayClosed"]),
        "Finalizer should close the gateway without flushing managed invoice writer.");
}

[MethodImpl(MethodImplOptions.NoInlining)]
static PaymentGatewayConnection CreateUndisposedSessionGateway(Action<string> resourceEventLogger)
{
    var session = new BillingRunSession(new MemoryStream(), resourceEventLogger);
    var gatewayConnection = session.GatewayConnection;

    try
    {
        session.RenewSubscription(new Subscription { Id = 3, RenewalAmount = -1 });
    }
    catch (PaymentGatewayException)
    {
    }

    return gatewayConnection;
}

static void RunBatchContinuesAndWritesOnlyCompleteInvoices()
{
    var failures = new List<Exception>();
    using var invoiceStream = new MemoryStream();
    using var session = new BillingRunSession(invoiceStream);
    var subscriptions = new List<Subscription>
    {
        new() { Id = 1, Status = Status.Active, DaysPastDue = 0, RenewalAmount = 99 },
        new() { Id = 2, Status = Status.Expired, DaysPastDue = 5, RenewalAmount = 199 },
        new() { Id = 3, Status = Status.Active, DaysPastDue = 0, RenewalAmount = -1 },
        new() { Id = 4, Status = Status.Cancelled, DaysPastDue = 0, RenewalAmount = 399 },
        new() { Id = 5, Status = Status.Active, DaysPastDue = 0, RenewalAmount = 299 }
    };

    session.ProcessRenewals(
        subscriptions,
        gracePeriodDays: 2,
        (_, exception) => failures.Add(exception));

    Assert(failures.Count == 3, "Batch should capture failed subscriptions and continue.");
    Assert(
        failures.Any(exception => exception is SubscriptionExpiredException),
        "Expired subscriptions past grace period should throw SubscriptionExpiredException.");
    Assert(
        failures.Any(exception => exception is PaymentGatewayException),
        "Declined renewal charges should throw PaymentGatewayException.");
    Assert(
        failures.Any(exception => exception is NotSupportedException),
        "Invalid status operations should throw NotSupportedException.");

    session.Dispose();

    invoiceStream.Position = 0;
    var invoiceLog = Encoding.UTF8.GetString(invoiceStream.ToArray());

    Assert(invoiceLog.Contains("Subscription=1;Amount=99"), "First valid invoice should be written.");
    Assert(invoiceLog.Contains("Subscription=5;Amount=299"), "Batch should continue to later valid subscriptions.");
    Assert(!invoiceLog.Contains("Subscription=2"), "Expired failed subscription should not write an invoice.");
    Assert(!invoiceLog.Contains("Subscription=3"), "Declined failed subscription should not write an invoice.");
    Assert(!invoiceLog.Contains("Subscription=4"), "Cancelled failed subscription should not write an invoice.");
}

static void Assert(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}
