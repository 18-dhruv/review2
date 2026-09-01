namespace SubscritionManager;

[AttributeUsage(AttributeTargets.Method)]
public class FeatureEntitlementAttribute : Attribute
{
    public FeatureEntitlementAttribute(string feature)
    {
        Feature = feature;
    }

    public string Feature { get; }
}
