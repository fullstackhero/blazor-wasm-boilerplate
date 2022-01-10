using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity;

public partial class Roles
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject]
    private IRolesClient RolesClient { get; set; } = default!;

    protected EntityClientTableContext<RoleDto, string?, RoleRequest> Context { get; set; } = default!;

    private bool _canViewRoleClaims;

    protected bool CheckBox { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canViewRoleClaims = (await AuthService.AuthorizeAsync(state.User, FSHPermissions.RoleClaims.View)).Succeeded;

        Context = new(
            fields: new()
            {
                new(role => role.Id, L["Id"]),
                new(role => role.Name, L["Name"]),
                new(role => role.Description, L["Description"])
            },
            idFunc: role => role.Id,
            loadDataFunc: async () => (await RolesClient.GetListAsync()).ToList(),
            searchFunc: Search,
            createFunc: async role => await RolesClient.RegisterRoleAsync(role),
            updateFunc: async (_, role) => await RolesClient.RegisterRoleAsync(role),
            deleteFunc: async id => await RolesClient.DeleteAsync(id),
            entityName: L["Role"],
            entityNamePlural: L["Roles"],
            searchPermission: FSHPermissions.Roles.ListAll,
            createPermission: FSHPermissions.Roles.Register,
            updatePermission: FSHPermissions.Roles.Update,
            deletePermission: FSHPermissions.Roles.Remove,
            hasExtraActionsFunc: () => _canViewRoleClaims,
            canUpdateEntityFunc: e => !e.IsDefault,
            canDeleteEntityFunc: e => !e.IsDefault);
    }

    private bool Search(string? searchString, RoleDto role) =>
        string.IsNullOrWhiteSpace(searchString)
        || role.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
        || role.Description?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true;

    private void ManagePermissions(string? roleId)
    {
        ArgumentNullException.ThrowIfNull(roleId, nameof(roleId));
        Navigation.NavigateTo($"/identity/role-permissions/{roleId}");
    }
}