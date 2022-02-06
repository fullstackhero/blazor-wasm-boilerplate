using FSH.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Auth;

public static class AuthorizationServiceExtensions
{
    public static async Task<bool> HasPermissionAsync(this IAuthorizationService service, ClaimsPrincipal user, string action, string resource) =>
        (await service.AuthorizeAsync(user, null, FSHPermission.NameFor(action, resource))).Succeeded;
}