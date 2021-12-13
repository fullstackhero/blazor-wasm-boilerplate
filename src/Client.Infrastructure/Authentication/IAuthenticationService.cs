using FSH.BlazorWebAssembly.Shared.Identity;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication;

public interface IAuthenticationService
{
    AuthProvider ProviderType { get; }
    Task<IResult> LoginAsync(TokenRequest model);
    Task<IResult> LogoutAsync();
    Task<IResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
}