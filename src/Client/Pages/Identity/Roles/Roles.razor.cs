using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Auth;
using FSH.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity.Roles;

public partial class Roles
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    private IRolesClient RolesClient { get; set; } = default!;

    protected EntityClientTableContext<RoleDto, string?, CreateOrUpdateRoleRequest> Context { get; set; } = default!;

    private bool _canViewRoleClaims;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canViewRoleClaims = await AuthService.HasPermissionAsync(state.User, FSHAction.View, FSHResource.RoleClaims);

        Context = new(
            entityName: L["Role"],
            entityNamePlural: L["Roles"],
            entityResource: FSHResource.Roles,
            searchAction: FSHAction.View,
            fields: new()
            {
                new(role => role.Id, L["Id"]),
                new(role => role.Name, L["Name"]),
                new(role => role.Description, L["Description"])
            },
            idFunc: role => role.Id,
            loadDataFunc: async () => (await RolesClient.GetListAsync()).ToList(),
            searchFunc: (searchString, role) =>
                string.IsNullOrWhiteSpace(searchString)
                    || role.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || role.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: async role => await RolesClient.RegisterRoleAsync(role),
            updateFunc: async (_, role) => await RolesClient.RegisterRoleAsync(role),
            deleteFunc: async id => await RolesClient.DeleteAsync(id),
            hasExtraActionsFunc: () => _canViewRoleClaims,
            canUpdateEntityFunc: e => !FSHRoles.IsDefault(e.Name),
            canDeleteEntityFunc: e => !FSHRoles.IsDefault(e.Name),
            exportAction: string.Empty);
    }

    private void ManagePermissions(string? roleId)
    {
        ArgumentNullException.ThrowIfNull(roleId, nameof(roleId));
        Navigation.NavigateTo($"/roles/{roleId}/permissions");
    }
}