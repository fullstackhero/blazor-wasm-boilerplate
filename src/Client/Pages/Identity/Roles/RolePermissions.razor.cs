using System.ComponentModel;
using System.Reflection;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

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

    private Dictionary<string, List<PermissionUpdateDto>> GroupedRoleClaims { get; } = new();

    public string _title = string.Empty;
    public string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditRoleClaims;
    private bool _canSearchRoleClaims;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canEditRoleClaims = (await AuthService.AuthorizeAsync(state.User, FSHPermissions.RoleClaims.Update)).Succeeded;
        _canSearchRoleClaims = (await AuthService.AuthorizeAsync(state.User, FSHPermissions.RoleClaims.View)).Succeeded;

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => RolesClient.GetByIdWithPermissionsAsync(Id), Snackbar)
            is RoleDto role)
        {
            _title = string.Format(L["{0} Permissions"], role.Name);
            _description = string.Format(L["Manage {0} Role Permissions"], role.Name);

            if (role.IsRootRole)
            {
                // Display Root Permissions only if the Role is Created for Root Tenant.

                var adminPermissions = DefaultPermissions.AdminPermissionTypes;
                GeneratePermissionGroups(adminPermissions, role);

                var rootPermissions = DefaultPermissions.RootPermissionTypes;
                GeneratePermissionGroups(rootPermissions, role);
            }
            else
            {
                var adminPermissions = DefaultPermissions.AdminPermissionTypes;
                GeneratePermissionGroups(adminPermissions, role);
            }
        }

        _loaded = true;
    }

    private void GeneratePermissionGroups(Type[] modules, RoleDto role)
    {
        List<PermissionUpdateDto> permissionListForModule = new();
        foreach (var module in modules)
        {
            permissionListForModule = new();
            string? moduleName = string.Empty;
            string? moduleDescription = string.Empty;

            if (module.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                .FirstOrDefault() is DisplayNameAttribute displayNameAttribute)
            {
                moduleName = displayNameAttribute.DisplayName;
            }

            if (module.GetCustomAttributes(typeof(DescriptionAttribute), true)
                .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
            {
                moduleDescription = descriptionAttribute.Description;
            }

            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var fi in fields)
            {
                object? propertyValue = fi.GetValue(null);

                if (propertyValue?.ToString() != null)
                {
                    permissionListForModule.Add(new PermissionUpdateDto(propertyValue.ToString() ?? string.Empty, role.Permissions?.Any(p => p.Permission == propertyValue.ToString()) is true)
                    {
                        Description = moduleDescription,
                        Group = moduleName
                    });
                }
            }

            GroupedRoleClaims.Add(L[moduleName], permissionListForModule);
        }
    }

    private Color GetGroupBadgeColor(int selected, int all)
    {
        if (selected == 0)
            return Color.Error;

        if (selected == all)
            return Color.Success;

        return Color.Info;
    }

    private async Task SaveAsync()
    {
        var allPermissions = GroupedRoleClaims.Values.SelectMany(a => a);
        var selectedPermissions = allPermissions.Where(a => a.Enabled);
        var request = new UpdatePermissionsRequest()
        {
            RoleId = Id,
            Permissions = selectedPermissions.Where(x => x.Enabled).Select(x => x.Permission).ToList(),
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => RolesClient.UpdatePermissionsAsync(request),
            Snackbar,
            null,
            L["Updated Permissions."]) is not null)
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