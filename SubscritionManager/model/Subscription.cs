namespace SubscritionManager;

public class Subscription:IEntity<int>
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Status Status { get; set; }
    public int DaysPastDue {get; set; }
    public int RenewalAmount { get; set; }
    
}

public enum Status
{
    Active,
    Expired,
    Cancelled
}
