using FSH.BlazorWebAssembly.Shared.Requests.Identity;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using System.Security.Claims;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Identity.Authentication
{
    public interface IAuthenticationService : IApiService
    {
        Task<IResult> Login(TokenRequest model);

        Task<IResult> Logout();

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

        Task<ClaimsPrincipal> CurrentUser();
    }
}