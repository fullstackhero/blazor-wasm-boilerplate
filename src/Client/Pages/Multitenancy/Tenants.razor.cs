using System.Security.Claims;
using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Multitenancy;

public partial class Tenants
{
    [Inject]
    private ITenantsClient TenantsClient { get; set; } = default!;
    private string? _searchString;
    protected EntityClientTableContext<TenantDetail, Guid, CreateTenantRequest> Context { get; set; } = default!;
    private List<TenantDetail> _tenants = new();
    public EntityTable<TenantDetail, Guid, CreateTenantRequest> EntityTable { get; set; } = default!;
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthorizeService { get; set; } = default!;
    private ClaimsPrincipal? _currentUser;

    private bool _canUpgrade;
    private bool _canModify;
    protected override async Task OnInitializedAsync()
    {
        Context = new(
            fields: new()
            {
                new(tenant => tenant.Id, L["Id"]),
                new(tenant => tenant.Name, L["Name"]),
                new(tenant => tenant.AdminEmail, L["Admin Email"]),
                new(tenant => tenant.ValidUpto.ToString("MMM dd, yyyy"), L["Valid Upto"]),
                new(tenant => tenant.IsActive, L["Active"], Type: typeof(bool))
            },
            loadDataFunc: async () => _tenants = (await TenantsClient.GetAllAsync()).Adapt<List<TenantDetail>>(),
            searchFunc: Search,
            createFunc: async tenant => await TenantsClient.CreateAsync(tenant.Adapt<CreateTenantRequest>()),
            searchPermission: true.ToString(),
            entityName: L["Tenant"],
            entityNamePlural: L["Tenants"],
            hasExtraActionsFunc: () => true,
            createPermission: FSHRootPermissions.Tenants.Create);
        var state = await AuthState;
        _currentUser = state.User;
        _canUpgrade = (await AuthorizeService.AuthorizeAsync(_currentUser, FSHRootPermissions.Tenants.UpgradeSubscription)).Succeeded;
        _canModify = (await AuthorizeService.AuthorizeAsync(_currentUser, FSHRootPermissions.Tenants.Update)).Succeeded;
    }

    private bool Search(string? searchString, TenantDetail tenantDto) =>
       string.IsNullOrWhiteSpace(searchString) || tenantDto?.Name?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true;

    private void ViewTenantDetails(string id)
    {
        var tenant = _tenants.First(f => f.Id == id);
        tenant.ShowDetails = !tenant.ShowDetails;
        foreach (var otherTenants in _tenants.Except(new[] { tenant }))
        {
            otherTenants.ShowDetails = false;
        }
    }

    private async Task ViewUpgradeSubscriptionModalAsync(string id)
    {
        var tenant = _tenants.First(f => f.Id == id);
        var parameters = new DialogParameters
        {
            {
                nameof(UpgradeSubscriptionModal.Request),
                new UpgradeSubscriptionRequest
                {
                    TenantId = tenant.Id,
                    ExtendedExpiryDate = tenant.ValidUpto
                }
            }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<UpgradeSubscriptionModal>(L["Upgrade Subscription"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    private async Task DeactivateTenantAsync(string id)
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => TenantsClient.DeactivateTenantAsync(id),
            Snackbar,
            null,
            L["Tenant Deactivated."]) is not null)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    private async Task ActivateTenantAsync(string id)
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => TenantsClient.ActivateTenantAsync(id),
            Snackbar,
            null,
            L["Tenant Activated."]) is not null)
        {
            await EntityTable.ReloadDataAsync();
        }
    }

    public class TenantDetail : TenantDto
    {
        public bool ShowDetails { get; set; }
    }
}