using System.Reflection;

namespace SubscritionManager;

public class DunningRetryPolicyService
{
    private static readonly int[] DefaultRetryDayOffsets = [1, 3, 7];
    private static readonly int[] PrioritySupportRetryDayOffsets = [1, 2, 3, 5, 7];
    private static readonly int[] EnterpriseRetryDayOffsets = [1, 2, 3, 5, 7, 10];
    private static readonly int[] EnterprisePrioritySupportRetryDayOffsets = [1, 2, 3, 5, 7, 10, 14];

    public int[] GetRetryDayOffsets(MethodInfo planProcessingMethod)
    {
        var planTier = planProcessingMethod.GetCustomAttribute<PlanTierAttribute>()?.Tier;
        var feature = planProcessingMethod.GetCustomAttribute<FeatureEntitlementAttribute>()?.Feature;

        var isEnterprise = string.Equals(planTier, "Enterprise", StringComparison.OrdinalIgnoreCase);
        var hasPrioritySupport = string.Equals(feature, "PrioritySupport", StringComparison.OrdinalIgnoreCase);

        if (isEnterprise && hasPrioritySupport) return EnterprisePrioritySupportRetryDayOffsets;
        if (isEnterprise) return EnterpriseRetryDayOffsets;
        if (hasPrioritySupport) return PrioritySupportRetryDayOffsets;

        return DefaultRetryDayOffsets;
    }

    public Predicate<Subscription> CreateDunningRuleFor(MethodInfo planProcessingMethod)
    {
        var retryDayOffsets = GetRetryDayOffsets(planProcessingMethod);

        return subscription => subscription.Status == Status.Expired
            && retryDayOffsets.Contains(subscription.DaysPastDue);
    }
}
