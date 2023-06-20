using FL_CRMS_ERP_WASM.Client.Components.EntityTable;
using FL_CRMS_ERP_WASM.Client.Infrastructure.ApiClient;
using FL_CRMS_ERP_WASM.Client.Infrastructure.Auth;
using FSH.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FL_CRMS_ERP_WASM.Client.Pages.Identity.Roles;

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
                new(role => role.Description, L["Description"]),
                new(role => role.ReportTo, L["ReportTo"])
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

        await GetAllRole();
    }

    private void ManagePermissions(string? roleId)
    {
        ArgumentNullException.ThrowIfNull(roleId, nameof(roleId));
        Navigation.NavigateTo($"/roles/{roleId}/permissions");
    }

    [Inject] IRolesClient _rolesClient { get; set; }
    List<RoleDto> _roleDtoList = new();

    async Task GetAllRole()
    {
        try
        {
            _roleDtoList = (await _rolesClient.GetListAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task<IEnumerable<string>> SearchRole(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
            return _roleDtoList.Select(x => x.Id);

        return _roleDtoList.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Id);
    }
}