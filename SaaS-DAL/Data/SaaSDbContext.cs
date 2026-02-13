using Microsoft.EntityFrameworkCore;
using SaaS_DAL.Data.InitDataFactory;
using SaaS_Domain.Entities;

namespace SaaS_DAL.Data;

public class SaaSDbContext : DbContext
{
    private readonly AbstractDataFactory? _factory;

    public SaaSDbContext(DbContextOptions<SaaSDbContext> options, AbstractDataFactory factory)
        : base(options)
    {
        this._factory = factory;
    }
    
    public SaaSDbContext(DbContextOptions<SaaSDbContext> options)
        : base(options)
    {
        this._factory = null;
    }
    
    public DbSet<Payment> Payments { get; set; }
    
    public DbSet<Plan> Plans { get; set; }
    
    public DbSet<PlanAddon> PlanAddons { get; set; }
    
    public DbSet<Subscription> Subscriptions { get; set; }
    
    public DbSet<SubscriptionAddon> SubscriptionAddons { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SaaSDbContext).Assembly);

        if (_factory == null) return;
        modelBuilder.Entity<Payment>().HasData(this._factory.GetPaymentData());
        modelBuilder.Entity<Plan>().HasData(this._factory.GetPlanData());
        modelBuilder.Entity<PlanAddon>().HasData(this._factory.GetPlanAddonData());
        modelBuilder.Entity<Subscription>().HasData(this._factory.GetSubscriptionData());
        modelBuilder.Entity<SubscriptionAddon>().HasData(this._factory.GetSubscriptionAddonData());
        modelBuilder.Entity<User>().HasData(this._factory.GetUserData());
        modelBuilder.Entity<UserRole>().HasData(this._factory.GetUserRoleData());
    }
}