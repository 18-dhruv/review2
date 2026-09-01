namespace SubscritionManager;

public class SubscriptionRuleService
{
    public Predicate<Subscription> CreateGracePeriodRule(int graceDays)
    {
        return subscription => subscription.Status == Status.Expired
            && subscription.DaysPastDue > graceDays;
    }

    public Predicate<Subscription> CreateDunningRule(int[] retryDayOffsets)
    {
        return subscription => subscription.Status == Status.Expired
            && retryDayOffsets.Contains(subscription.DaysPastDue);
    }
}
