using FSH.BlazorWebAssembly.Shared.Requests.Identity;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication;

public interface IAuthenticationService
{
    AuthProvider ProviderType { get; }
    Task<IResult> Login(TokenRequest model);
    Task<IResult> Logout();
    Task<string> RefreshToken();
    Task<string> TryRefreshToken();
    Task<string> TryForceRefreshToken();
}