using FSH.BlazorWebAssembly.Shared.Identity;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;

    public IdentityService(HttpClient httpClient) =>
        _httpClient = httpClient;

    public async Task<IResult<UserDetailsDto>> GetProfileDetailsAsync()
    {
        var response = await _httpClient.GetAsync(IdentityEndpoints.Profile);
        return await response.ToResultAsync<UserDetailsDto>();
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest model)
    {
        var response = await _httpClient.PostAsJsonAsync(IdentityEndpoints.ResetPassword, model);
        return await response.ToResultAsync();
    }
}
