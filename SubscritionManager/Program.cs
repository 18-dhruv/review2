using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SubscritionManager;
using SubscritionManager.repository;

using (var dbContext = new SubscriptionDbContext())
{

    dbContext.Database.EnsureCreated();
    SeedSubscriptions(dbContext);

    var subscriptions = dbContext.Subscriptions
        .AsNoTracking()
        .OrderBy(subscription => subscription.Id)
        .ToList();

    var subscriptionPlanTiers = new Dictionary<int, PlanTier>
    {
        [1] = PlanTier.Basic,
        [2] = PlanTier.Standard,
        [3] = PlanTier.Standard,
        [4] = PlanTier.Gold,
        [5] = PlanTier.Gold
    };

    Console.WriteLine($"DbContext provider: {dbContext.Database.ProviderName}");
    Console.WriteLine($"Loaded {subscriptions.Count} subscriptions from database.");
    Console.WriteLine();

    PrintAnalytics(subscriptions, subscriptionPlanTiers);
    PrintDunningRetryPolicy();
    RunNotifications(subscriptions[0], subscriptions[1]);
    RunBillingBatch(subscriptions);

    static void SeedSubscriptions(SubscriptionDbContext dbContext)
    {
        if (dbContext.Subscriptions.Any()) return;

        dbContext.Subscriptions.AddRange(
            new Subscription { Id = 1, CustomerId = 101, Status = Status.Active, DaysPastDue = 0, RenewalAmount = 99 },
            new Subscription
                { Id = 2, CustomerId = 102, Status = Status.Expired, DaysPastDue = 5, RenewalAmount = 199 },
            new Subscription { Id = 3, CustomerId = 103, Status = Status.Active, DaysPastDue = 0, RenewalAmount = -1 },
            new Subscription
                { Id = 4, CustomerId = 104, Status = Status.Cancelled, DaysPastDue = 0, RenewalAmount = 399 },
            new Subscription
                { Id = 5, CustomerId = 105, Status = Status.Active, DaysPastDue = 0, RenewalAmount = 299 });

        dbContext.SaveChanges();
    }

    static void PrintAnalytics(
        IEnumerable<Subscription> subscriptions,
        Dictionary<int, PlanTier> subscriptionPlanTiers)
    {
        var analyticsService = new SubscriptionAnalyticsService();
        var report = analyticsService.AnalyzeByPlanTier(
            subscriptions,
            subscription => subscriptionPlanTiers[subscription.Id]);

        Console.WriteLine("Subscription analytics");
        Console.WriteLine(analyticsService.FormatReport(report));
        Console.WriteLine();
    }

    static void PrintDunningRetryPolicy()
    {
        var processingMethod = typeof(SubscriptionProcessingService).GetMethod(
            nameof(SubscriptionProcessingService.ProcessEnterprisePrioritySupportPlan),
            BindingFlags.Instance | BindingFlags.Public);

        if (processingMethod == null)
            throw new MissingMethodException(nameof(SubscriptionProcessingService),
                nameof(SubscriptionProcessingService.ProcessEnterprisePrioritySupportPlan));

        var dunningRetryPolicyService = new DunningRetryPolicyService();
        var retryDayOffsets = dunningRetryPolicyService.GetRetryDayOffsets(processingMethod);

        Console.WriteLine("Dunning retry policy");
        Console.WriteLine($"Enterprise PrioritySupport retry days: {string.Join(", ", retryDayOffsets)}");
        Console.WriteLine();
    }

    static void RunNotifications(
        Subscription renewedSubscription,
        Subscription cancellationRiskSubscription)
    {
        var notificationService = new SubscriptionNotificationService();

        notificationService.SubscribeRevenueLedger(subscription =>
            Console.WriteLine($"Revenue ledger updated for subscription {subscription.Id}."));

        notificationService.SubscribeRetentionQueue(subscription =>
            Console.WriteLine($"Retention queue updated for subscription {subscription.Id}."));

        Console.WriteLine("Notifications");
        notificationService.RaiseSubscriptionRenewed(renewedSubscription);
        notificationService.RaiseCancellationRiskDetected(cancellationRiskSubscription);
        Console.WriteLine();
    }

    static void RunBillingBatch(IEnumerable<Subscription> subscriptions)
    {
        var resourceEvents = new List<string>();
        var failedRenewals = new List<string>();

        using var invoiceStream = new MemoryStream();
        using (var billingRunSession = new BillingRunSession(invoiceStream, resourceEvents.Add))
        {
            billingRunSession.ProcessRenewals(
                subscriptions,
                gracePeriodDays: 2,
                (subscription, exception) =>
                    failedRenewals.Add(
                        $"Subscription {subscription.Id}: {exception.GetType().Name} - {exception.Message}"));
        }

        invoiceStream.Position = 0;
        var invoiceLog = Encoding.UTF8.GetString(invoiceStream.ToArray());

        Console.WriteLine("Billing run");
        Console.WriteLine("Invoice log");
        Console.WriteLine(string.IsNullOrWhiteSpace(invoiceLog) ? "No invoices written." : invoiceLog.Trim());
        Console.WriteLine();

        Console.WriteLine("Failed renewals");
        foreach (var failedRenewal in failedRenewals)
            Console.WriteLine(failedRenewal);

        Console.WriteLine();
        Console.WriteLine("Resource events");
        foreach (var resourceEvent in resourceEvents)
            Console.WriteLine(resourceEvent);
    }
}
