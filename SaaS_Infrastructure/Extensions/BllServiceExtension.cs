using Microsoft.Extensions.DependencyInjection;
using SaaS_BLL.Interfaces;
using SaaS_BLL.Mappings;
using SaaS_BLL.Services;

namespace SaaS_Infrastructure.Extensions;

public static class BllServiceExtensions
{
    public static IServiceCollection AddBll(this IServiceCollection services)
    {
        // Register AutoMapper with all profiles from this assembly
        services.AddAutoMapper(cfg => cfg.AddProfile<SaaSMappingProfile>());

        // Register services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IPlanService, PlanService>();
        services.AddScoped<IPlanAddonService, PlanAddonService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<ISubscriptionAddonService, SubscriptionAddonService>();
        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}