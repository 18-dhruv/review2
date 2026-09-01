namespace SubscritionManager;

public class PaymentGatewayConnection
{
    private readonly Action<string>? _resourceEventLogger;

    public PaymentGatewayConnection(Action<string>? resourceEventLogger = null)
    {
        _resourceEventLogger = resourceEventLogger;
        IsOpen = true;
    }

    public bool IsOpen { get; private set; }
    public bool IsClosed { get; private set; }

    public void ChargeRenewal(Subscription subscription)
    {
        if (!IsOpen) throw new InvalidOperationException("Payment gateway connection is closed.");
        if (subscription.RenewalAmount < 0) throw new PaymentGatewayException("Renewal charge was declined.");
    }

    public void Close()
    {
        if (IsClosed) return;

        IsOpen = false;
        IsClosed = true;
        _resourceEventLogger?.Invoke("GatewayClosed");
    }
}
