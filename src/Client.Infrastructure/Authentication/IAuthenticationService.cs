using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication;

public interface IAuthenticationService
{
    AuthProvider ProviderType { get; }
    Task<IResult> LoginAsync(string tenantKey, TokenRequest request);
    Task<IResult> LogoutAsync();
    Task<IResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
}