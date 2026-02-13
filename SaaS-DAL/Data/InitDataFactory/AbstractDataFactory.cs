using SaaS_Domain.Entities;

namespace SaaS_DAL.Data.InitDataFactory;

public abstract class AbstractDataFactory
{
    public abstract Payment[] GetPaymentData();

    public abstract Plan[] GetPlanData();
    
    public abstract PlanAddon[] GetPlanAddonData();
    
    public abstract Subscription[] GetSubscriptionData();
    
    public abstract SubscriptionAddon[] GetSubscriptionAddonData();
    
    public abstract User[] GetUserData();
    
    public abstract UserRole[] GetUserRoleData();
}