using FSH.BlazorWebAssembly.Shared.Requests.Identity;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Services.Identity.Authentication
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