using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Mapster;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity.Roles;

public partial class RolePermissions
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Parameter]
    public string? Id { get; set; }
    [Parameter]
    public string? Title { get; set; }
    [Parameter]
    public string? Description { get; set; }
    [Inject]
    protected IRolesClient RolesClient { get; set; } = default!;
    public List<PermissionUpdateDto> RolePermissionsList { get; set; } = new();

    private string _searchString = string.Empty;
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;

    private bool _canEditRoleClaims;
    private bool _canSearchRoleClaims;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canEditRoleClaims = (await AuthService.AuthorizeAsync(state.User, FSHPermissions.RoleClaims.Edit)).Succeeded;
        _canSearchRoleClaims = (await AuthService.AuthorizeAsync(state.User, FSHPermissions.RoleClaims.View)).Succeeded;

        var rolePermissions = new List<PermissionUpdateDto>();

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => RolesClient.GetByIdWithPermissionsAsync(Id), Snackbar)
            is RoleDto role)
        {
            Title = role.Name;
            Description = string.Format(_localizer["Manage {0}'s Permissions"], role.Name);

            rolePermissions = role.Permissions?.Adapt<List<PermissionUpdateDto>>();
        }

        var allPermissions = DefaultPermissions.Admin;
        allPermissions.AddRange(DefaultPermissions.Root);
        var result = allPermissions.Select(x => new PermissionUpdateDto()
        {
            Permission = x
        }).ToList();

        foreach (var permission in result)
        {
            if (rolePermissions.Select(z => z.Permission).Contains(permission.Permission))
                permission.Enabled = true;
        }

        RolePermissionsList = result;

        _loaded = true;
    }

    private async Task SaveAsync()
    {
        var request = new UpdatePermissionsRequest()
        {
            Permissions = RolePermissionsList.Where(x => x.Enabled).Select(x => x.Permission).ToList(),
            RoleId = Id
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

    private bool Search(PermissionDto permission)
    {
        if (string.IsNullOrWhiteSpace(_searchString)) return true;
        if (permission.Permission?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }

    public class PermissionUpdateDto : PermissionDto
    {
        public bool Enabled { get; set; }
    }
}