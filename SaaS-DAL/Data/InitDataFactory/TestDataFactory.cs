using SaaS_Domain.Entities;

namespace SaaS_DAL.Data.InitDataFactory;

public class TestDataFactory : AbstractDataFactory
{
    public override Payment[] GetPaymentData()
    {
        return Array.Empty<Payment>();
    }

    public override Plan[] GetPlanData()
    {
        return Array.Empty<Plan>();
    }

    public override PlanAddon[] GetPlanAddonData()
    {
        return Array.Empty<PlanAddon>();
    }
    
    public override Subscription[] GetSubscriptionData()
    {
        return Array.Empty<Subscription>();
    }

    public override SubscriptionAddon[] GetSubscriptionAddonData()
    {
        return Array.Empty<SubscriptionAddon>();
    }

    public override User[] GetUserData()
    {
        return Array.Empty<User>();
    }

    public override UserRole[] GetUserRoleData()
    {
        return new[]
        {
            new UserRole(1, "Admin", "role for managing things..."),
            new UserRole(2, "User", "Just a usual subscriber")
        };
    }
}