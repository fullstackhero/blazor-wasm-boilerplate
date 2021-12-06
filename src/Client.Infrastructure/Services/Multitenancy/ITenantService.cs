using FSH.BlazorWebAssembly.Shared.Multitenancy;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Multitenancy;

public interface ITenantService : IApiService
{
    Task<IResult<List<TenantDto>>> GetAllAsync();
    Task<IResult> CreateAsync(CreateTenantRequest request);
    Task<IResult> UpgradeAsync(UpgradeSubscriptionRequest request);
    Task<IResult> DeactivateTenantAsync(string id);
    Task<IResult> ActivateTenantAsync(string id);
}