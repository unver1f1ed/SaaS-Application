namespace SaaS_BLL.Models;

public record SubscriptionAddonDto(
    int Id,
    int SubscriptionId,
    int PlanAddonId,
    string AddonName,
    int Quantity,
    DateTime AddedDate);