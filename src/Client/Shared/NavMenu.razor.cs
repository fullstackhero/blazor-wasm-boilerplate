using FSH.BlazorWebAssembly.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FSH.BlazorWebAssembly.Client.Shared;

public partial class NavMenu
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    private ClaimsPrincipal? _currentUser;

    private bool _canViewHangfire;
    private bool _canViewDashboard;
    private bool _canViewAuditLogs;
    private bool _canViewRoles;
    private bool _canViewUsers;
    private bool _canViewProducts;
    private bool _canViewBrands;
    private bool _canViewTenants;
    private bool CanViewAdministrationGroup => _canViewUsers || _canViewRoles || _canViewTenants;
    protected override async Task OnParametersSetAsync()
    {
        var state = await AuthState;
        _currentUser = state.User;
        _canViewHangfire = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Hangfire.View)).Succeeded;
        _canViewDashboard = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Dashboard.View)).Succeeded;
        _canViewAuditLogs = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.AuditLogs.View)).Succeeded;
        _canViewRoles = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Roles.View)).Succeeded;
        _canViewUsers = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Users.View)).Succeeded;
        _canViewProducts = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Products.View)).Succeeded;
        _canViewBrands = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Brands.View)).Succeeded;
        _canViewTenants = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Tenants.View)).Succeeded;
    }
}
