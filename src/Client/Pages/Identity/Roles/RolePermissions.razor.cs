using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity.Roles;

public partial class RolePermissions
{
    [Parameter]
    public string Id { get; set; } = default!; // from route
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    protected IRolesClient RolesClient { get; set; } = default!;

    protected List<PermissionUpdateDto> RolePermissionsList { get; set; } = new();

    public string _title = string.Empty;
    public string _description = string.Empty;

    private string _searchString = string.Empty;
    private bool _dense;
    private bool _striped = true;
    private bool _bordered;

    private bool _canEditRoleClaims;
    private bool _canSearchRoleClaims;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canEditRoleClaims = (await AuthService.AuthorizeAsync(state.User, FSHPermissions.RoleClaims.Edit)).Succeeded;
        _canSearchRoleClaims = (await AuthService.AuthorizeAsync(state.User, FSHPermissions.RoleClaims.View)).Succeeded;

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => RolesClient.GetByIdWithPermissionsAsync(Id), Snackbar)
            is RoleDto role)
        {
            _title = string.Format(_localizer["{0} Permissions"], role.Name);
            _description = string.Format(_localizer["Manage {0} Role Permissions"], role.Name);

            RolePermissionsList = DefaultPermissions.Admin
                .Union(DefaultPermissions.Root)
                .Select(permission => new PermissionUpdateDto(
                    permission,
                    role.Permissions?.Any(p => p.Permission == permission) is true))
                .ToList();
        }

        _loaded = true;
    }

    private async Task SaveAsync()
    {
        var request = new UpdatePermissionsRequest()
        {
            RoleId = Id,
            Permissions = RolePermissionsList.Where(x => x.Enabled).Select(x => x.Permission).ToList(),
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => RolesClient.UpdatePermissionsAsync(request),
            Snackbar,
            null,
            _localizer["Success"]) is not null)
        {
            Navigation.NavigateTo("/roles");
        }
    }

    private bool Search(PermissionDto permission) =>
        string.IsNullOrWhiteSpace(_searchString)
            || permission.Permission?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true;

    public class PermissionUpdateDto : PermissionDto
    {
        public bool Enabled { get; set; }

        public PermissionUpdateDto(string permission, bool enabled) =>
            (Permission, Enabled) = (permission, enabled);
    }
}