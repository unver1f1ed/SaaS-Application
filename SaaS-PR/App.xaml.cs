namespace SaaS_PR;

using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaaS_Infrastructure.Extensions;
using Core;
using Extensions;
using ViewModels.Auth;

public partial class App
{
    private ServiceProvider _serviceProvider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        this._serviceProvider = services.BuildServiceProvider();

        // Composition root — only acceptable place for direct container resolution
        this._serviceProvider.GetRequiredService<RootNavigationService>()
            .NavigateTo<LoginViewModel>();

        this._serviceProvider.GetRequiredService<MainWindow>().Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        this._serviceProvider.Dispose();
        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddInfrastructure(configuration);
        services.AddPr();
        services.AddSingleton<MainWindow>();
    }
}