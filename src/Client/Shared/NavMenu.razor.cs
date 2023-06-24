using FL_CRMS_ERP_WASM.Client.Infrastructure.Auth;
using FL_CRMS_ERP_WASM.Client.Infrastructure.Common;
using FL.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FL_CRMS_ERP_WASM.Client.Shared;

public partial class NavMenu
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Parameter]
    public bool _drawerOpen { get; set; }
    private string? _hangfireUrl;
    private bool _canViewHangfire;
    private bool _canViewDashboard;
    private bool _canViewRoles;
    private bool _canViewUsers;
    private bool _canViewProducts;
    private bool _canViewBrands;
    private bool _canViewTenants;
    private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles || _canViewTenants;

    protected override async Task OnParametersSetAsync()
    {
        _hangfireUrl = Config[ConfigNames.ApiBaseUrl] + "jobs";
        var user = (await AuthState).User;
        _canViewHangfire = await AuthService.HasPermissionAsync(user, FLAction.View, FLResource.Hangfire);
        _canViewDashboard = await AuthService.HasPermissionAsync(user, FLAction.View, FLResource.Dashboard);
        _canViewRoles = await AuthService.HasPermissionAsync(user, FLAction.View, FLResource.Roles);
        _canViewUsers = await AuthService.HasPermissionAsync(user, FLAction.View, FLResource.Users);
        _canViewProducts = await AuthService.HasPermissionAsync(user, FLAction.View, FLResource.Products);
        _canViewBrands = await AuthService.HasPermissionAsync(user, FLAction.View, FLResource.Brands);
        _canViewTenants = await AuthService.HasPermissionAsync(user, FLAction.View, FLResource.Tenants);
    }
}