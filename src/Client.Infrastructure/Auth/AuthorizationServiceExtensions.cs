using FL.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace FL_CRMS_ERP_WASM.Client.Infrastructure.Auth;

public static class AuthorizationServiceExtensions
{
    public static async Task<bool> HasPermissionAsync(this IAuthorizationService service, ClaimsPrincipal user, string action, string resource) =>
        (await service.AuthorizeAsync(user, null, FLPermission.NameFor(action, resource))).Succeeded;
}