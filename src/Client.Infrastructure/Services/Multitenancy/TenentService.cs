using FSH.BlazorWebAssembly.Shared.Multitenancy;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Multitenancy;
public class TenentService : ITenentService
{
    private readonly HttpClient _httpClient;

    public TenentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult<List<TenantDto>>> GetAllAsync()
    {
        var response = await _httpClient.GetAsync(TenentEndpoints.GetAll);
        return await response.ToResult<List<TenantDto>>();
    }

    public Task<IResult> CreateAsync(CreateTenantRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> UpgradeAsync(UpgradeSubscriptionRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> DeactivateTenantAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IResult> ActivateTenantAsync(string id)
    {
        throw new NotImplementedException();
    }
}