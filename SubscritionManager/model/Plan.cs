namespace SubscritionManager;

public class Plan:IEntity<int>
{
    public int Id { get; set; }
    public PlanTier PlanTier { get; set;}

    private Plan()
    {
    }

    public int GracePeriodDays
    {
        get
        {
            return 2;
        }
    }

    public int Price { 
        get
        {
            if (this.PlanTier == PlanTier.Basic) return 99;
            if (this.PlanTier == PlanTier.Standard) return 199;
            if (this.PlanTier == PlanTier.Gold) return 399;
            return 0;
        }
    }

    public Plan(PlanTier planTier,int gracePeriodDays)
    {
        this.PlanTier = planTier;
    }
    
}

public enum PlanTier
{
    Basic,
    Standard,
    Gold
}
