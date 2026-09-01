namespace SubscritionManager;

[AttributeUsage(AttributeTargets.Method)]
public class PlanTierAttribute : Attribute
{
    public PlanTierAttribute(string tier)
    {
        Tier = tier;
    }

    public string Tier { get; }
}
