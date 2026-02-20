namespace SaaS_PR;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SaaS_DAL.Data;
using SaaS_DAL.Data.InitDataFactory;
using SaaS_DAL.Interfaces;
using SaaS_DAL.Repository;

public partial class App : Application
{
    private ServiceProvider _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        
        _serviceProvider = serviceCollection.BuildServiceProvider();
        
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var dataFactory = new ReleaseDataFactory();
        var dbPath = "DB/SaaS.db";
        
        // Register data factory
        services.AddSingleton<AbstractDataFactory>(dataFactory);
        
        // Register DbContext
        var dbFactory = new SaaSDbFactory(dataFactory, dbPath);
        services.AddScoped<SaaSDbContext>(provider => dbFactory.CreateDbContext());
        
        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPlanRepository, PlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPlanAddonRepository, PlanAddonRepository>();
        services.AddScoped<ISubscriptionAddonRepository, SubscriptionAddonRepository>();
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Register BLL Services TODO
        // services.AddScoped<IUserService, UserService>(); TODO
        // services.AddScoped<ISubscriptionService, SubscriptionService>(); TODO
        
        // Register Views TODO
        services.AddTransient<MainWindow>();
    }
}