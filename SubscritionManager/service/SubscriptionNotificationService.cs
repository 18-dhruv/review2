namespace SubscritionManager;

public class SubscriptionNotificationService
{
    public event Action<Subscription>? SubscriptionRenewed;
    public event Action<Subscription>? CancellationRiskDetected;

    public void SubscribeRevenueLedger(Action<Subscription> revenueLedgerHandler)
    {
        SubscriptionRenewed += revenueLedgerHandler;
    }

    public void SubscribeRetentionQueue(Action<Subscription> retentionQueueHandler)
    {
        CancellationRiskDetected += retentionQueueHandler;
    }

    public void RaiseSubscriptionRenewed(Subscription subscription)
    {
        SubscriptionRenewed?.Invoke(subscription);
    }

    public void RaiseCancellationRiskDetected(Subscription subscription)
    {
        CancellationRiskDetected?.Invoke(subscription);
    }
}
