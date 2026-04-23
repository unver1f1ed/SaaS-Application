using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SaaS_Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
                               ?? throw new InvalidOperationException(
                                   "Connection string 'Default' not found in configuration.");

        var isDevelopment = configuration.GetValue<bool>("IsDevelopment");

        services
            .AddDal(isDevelopment, connectionString)
            .AddBll();

        return services;
    }
}