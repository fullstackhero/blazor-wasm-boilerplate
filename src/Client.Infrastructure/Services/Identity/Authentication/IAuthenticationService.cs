using FSH.BlazorWebAssembly.Shared.Requests.Identity;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Identity.Authentication;

public interface IAuthenticationService : IApiService
{
    Task<IResult> Login(TokenRequest model);

    Task<IResult> Logout();

    Task<string> RefreshToken();

    Task<string> TryRefreshToken();

    Task<string> TryForceRefreshToken();

    Task<ClaimsPrincipal> CurrentUser();
}