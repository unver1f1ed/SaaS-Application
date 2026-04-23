using Microsoft.Extensions.DependencyInjection;
using SaaS_PR.Core;
using SaaS_PR.ViewModels.Admin;
using SaaS_PR.ViewModels.Auth;
using SaaS_PR.ViewModels.User;

namespace SaaS_PR.Extensions;

public static class PrServiceExtensions
{
    public static IServiceCollection AddPr(this IServiceCollection services)
    {
        // Core
        services.AddSingleton<SessionContext>();
        services.AddSingleton<ViewLocator>();

        // Factory delegate — only acceptable use of IServiceProvider outside composition root
        services.AddSingleton<Func<Type, ViewModelBase>>(sp =>
            type => (ViewModelBase)sp.GetRequiredService(type));

        // Two separate navigation services — root drives MainWindow,
        // shell drives the content area inside each shell
        services.AddSingleton<RootNavigationService>();
        services.AddSingleton<ShellNavigationService>();

        // Auth ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();

        // Admin ViewModels
        services.AddTransient<AdminShellViewModel>();
        services.AddTransient<AdminDashboardViewModel>();
        services.AddTransient<UsersViewModel>();
        services.AddTransient<PlansViewModel>();
        services.AddTransient<PlanAddonsViewModel>();
        services.AddTransient<SubscriptionsViewModel>();
        services.AddTransient<SubscriptionAddonsViewModel>();
        services.AddTransient<PaymentsViewModel>();
        services.AddTransient<AdminProfileViewModel>();

        // User ViewModels
        services.AddTransient<UserShellViewModel>();
        services.AddTransient<UserDashboardViewModel>();
        services.AddTransient<BrowsePlansViewModel>();
        services.AddTransient<MySubscriptionViewModel>();
        services.AddTransient<MyPaymentsViewModel>();
        services.AddTransient<UserProfileViewModel>();

        return services;
    }
}