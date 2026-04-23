using Microsoft.Extensions.DependencyInjection;
using SaaS_DAL.Data;
using SaaS_DAL.Data.InitDataFactory;
using SaaS_DAL.Repository;
using SaaS_Domain.Interfaces;

namespace SaaS_Infrastructure.Extensions;

public static class DalServiceExtensions
{
    public static IServiceCollection AddDal(
        this IServiceCollection services,
        bool isDevelopment,
        string connectionString)
    {
        AbstractDataFactory dataFactory = isDevelopment
            ? new TestDataFactory()
            : new ReleaseDataFactory();

        // Register data factory
        services.AddSingleton<AbstractDataFactory>(dataFactory);

        // Register DbContext via runtime factory
        var dbFactory = new SaaSDbFactory(dataFactory, connectionString);
        services.AddSingleton<SaaSDbFactory>(dbFactory);
        services.AddScoped<SaaSDbContext>(provider =>
            provider.GetRequiredService<SaaSDbFactory>().CreateDbContext());

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPlanAddonRepository, PlanAddonRepository>();
        services.AddScoped<ISubscriptionAddonRepository, SubscriptionAddonRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}