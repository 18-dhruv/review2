namespace SubscritionManager;

public class SubscriptionProcessingService
{
    [PlanTier("Enterprise")]
    [FeatureEntitlement("PrioritySupport")]
    public void ProcessEnterprisePrioritySupportPlan(
        Subscription subscription,
        Action<Subscription> processingHandler)
    {
        processingHandler(subscription);
    }

    public List<Subscription> FilterDueForProcessing(
        IEnumerable<Subscription> subscriptions,
        Predicate<Subscription> dueForProcessingRule)
    {
        return subscriptions.Where(subscription => dueForProcessingRule(subscription))
                            .ToList();
    }

    public void NotifyDueForProcessing(
        IEnumerable<Subscription> subscriptions,
        Predicate<Subscription> dueForProcessingRule,
        Action<Subscription> notificationHandler)
    {
        foreach (var subscription in subscriptions)
        {
            if (dueForProcessingRule(subscription)) notificationHandler(subscription);
        }
    }
}
