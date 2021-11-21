using FSH.BlazorWebAssembly.Shared.Identity;
using System.Collections.Generic;

namespace FSH.BlazorWebAssembly.Shared.Requests.Identity
{
    public class UserRolesRequest
    {
        public List<UserRoleDto> UserRoles { get; set; } = new();
    }
}