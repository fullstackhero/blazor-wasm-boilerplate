using FL.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace FL_CRMS_ERP_WASM.Client.Infrastructure.Auth;

public class MustHavePermissionAttribute : AuthorizeAttribute
{
    public MustHavePermissionAttribute(string action, string resource) =>
        Policy = FLPermission.NameFor(action, resource);
}