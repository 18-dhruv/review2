using System.Globalization;

namespace SubscritionManager;

public class SubscriptionAnalyticsService
{
    
    public SubscriptionAnalyticsReport AnalyzeByPlanTier(IEnumerable<Subscription> subscriptions, Func<Subscription, PlanTier> planTierSelector)
    {
        var tierSummaries = subscriptions
            .GroupBy(planTierSelector)
            .Select(group =>
            {
                var totalSubscriptions = group.Count();
                var expiredSubscriptions = group.Count(subscription => subscription.Status == Status.Expired);
                var pastDueSubscriptions = group.Count(subscription => subscription.DaysPastDue > 0);
                var monthlyRecurringRevenue = group
                    .Where(subscription => subscription.Status == Status.Active)
                    .Sum(subscription => subscription.RenewalAmount);

                return new PlanTierAnalytics(group.Key, totalSubscriptions, expiredSubscriptions, pastDueSubscriptions, CalculateRate(expiredSubscriptions, totalSubscriptions), monthlyRecurringRevenue, CalculateRate(pastDueSubscriptions, totalSubscriptions));
                
            }).ToList();

        return new SubscriptionAnalyticsReport(tierSummaries, tierSummaries.OrderByDescending(summary => summary.PastDueRate).FirstOrDefault());
    }

    
    public SubscriptionAnalyticsReport AnalyzeByPlanTier(IRepository<int, Subscription> subscriptionRepository, Func<Subscription, PlanTier> planTierSelector)
    {
        var subscriptions = subscriptionRepository
            .GetAll()
            .Select(subscriptionEntry => subscriptionEntry.Value);

        return AnalyzeByPlanTier(subscriptions, planTierSelector);
    }

    public string FormatReport(SubscriptionAnalyticsReport report, IFormatProvider? formatProvider = null)
    {
        var provider = formatProvider ?? CultureInfo.CurrentCulture;
        var tierLines = report.TierSummaries
            .OrderBy(summary => summary.PlanTier)
            .Select(summary => FormatTierSummary(summary, provider));

        var highestPastDueTier = report.HighestPastDueTier == null
            ? "Highest past-due tier: none"
            : $"Highest past-due tier: {report.HighestPastDueTier.PlanTier} ({report.HighestPastDueTier.PastDueRate.ToString("P2", provider)})";

        return string.Join(
            Environment.NewLine,
            tierLines.Append(highestPastDueTier));
    }

    private static decimal CalculateRate(int part, int total)
    {
        return total == 0 ? 0 : (decimal)part / total;
    }

    private static string FormatTierSummary(PlanTierAnalytics summary, IFormatProvider provider)
    {
        return $"{summary.PlanTier}: " + $"Total={summary.TotalSubscriptions}, " + $"Expired={summary.ExpiredSubscriptions}, " + $"PastDue={summary.PastDueSubscriptions}, " + $"Churn={summary.ChurnRate.ToString("P2", provider)}, " + $"MRR={summary.MonthlyRecurringRevenue.ToString("C0", provider)}, " + $"PastDueRate={summary.PastDueRate.ToString("P2", provider)}";
    }
}

public record SubscriptionAnalyticsReport(List<PlanTierAnalytics> TierSummaries, PlanTierAnalytics? HighestPastDueTier);

public record PlanTierAnalytics(PlanTier PlanTier, int TotalSubscriptions, int ExpiredSubscriptions, int PastDueSubscriptions, decimal ChurnRate, int MonthlyRecurringRevenue, decimal PastDueRate);
