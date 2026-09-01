namespace SubscritionManager;

public class SubscriptionExpiredException : Exception
{
    public SubscriptionExpiredException(int subscriptionId, int daysPastDue)
        : base($"Subscription {subscriptionId} expired after {daysPastDue} days past due.")
    {
        SubscriptionId = subscriptionId;
        DaysPastDue = daysPastDue;
    }

    public SubscriptionExpiredException(int subscriptionId, int daysPastDue, string message)
        : base(message)
    {
        SubscriptionId = subscriptionId;
        DaysPastDue = daysPastDue;
    }

    public SubscriptionExpiredException(
        int subscriptionId,
        int daysPastDue,
        string message,
        Exception innerException)
        : base(message, innerException)
    {
        SubscriptionId = subscriptionId;
        DaysPastDue = daysPastDue;
    }

    public int SubscriptionId { get; }
    public int DaysPastDue { get; }
}
