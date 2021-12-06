namespace FSH.BlazorWebAssembly.Shared.Multitenancy;

public class UpgradeSubscriptionRequest
{
    public string Tenant { get; set; }
    public DateTime ExtendedExpiryDate { get; set; }
}