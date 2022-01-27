using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Multitenancy;

public partial class Tenants
{
    [Inject]
    private ITenantsClient TenantsClient { get; set; } = default!;
    [Inject]
    private IDialogService _dialogService{ get; set; } = default!;
    private string? _searchString;
    protected EntityClientTableContext<TenantDetail, Guid, CreateTenantRequest> Context { get; set; } = default!;
    private List<TenantDetail> _tenants = new();
    public EntityTable<TenantDetail, Guid, CreateTenantRequest> entityTable { get; set; }
    protected override void OnInitialized() =>
        Context = new(
            fields: new()
            {
                new(tenant => tenant.Id, L["TenantId"]),
                new(tenant => tenant.Name, L["Name"]),
                new(tenant => tenant.AdminEmail, L["Admin Email"]),
                new(tenant => tenant.ValidUpto.ToString("MMM dd, yyyy"), L["Valid Upto"]),
                new(tenant => tenant.IsActive, L["Active"], Type: typeof(bool))
            },
            loadDataFunc: async () => _tenants = (await TenantsClient.GetAllAsync()).Adapt<List<TenantDetail>>(),
            searchFunc: Search,
            createFunc: async tenant => await TenantsClient.CreateAsync(tenant.Adapt<CreateTenantRequest>()),
            searchPermission: true.ToString(),
            entityNamePlural: L["Tenants"],
            hasExtraActionsFunc: () => true,
            createPermission: FSHPermissions.Tenants.Register);

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
        var parameters = new DialogParameters();
        parameters.Add(nameof(UpgradeSubscriptionModal.Request), new UpgradeSubscriptionRequest
        {
            TenantId = tenant.Id,
            ExtendedExpiryDate = tenant.ValidUpto
        });
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<UpgradeSubscriptionModal>(L["Upgrade Subscription"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await entityTable.ReloadDataAsync();
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
            await entityTable.ReloadDataAsync();
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
            await entityTable.ReloadDataAsync();
        }
    }

    public class TenantDetail : TenantDto
    {
        public bool ShowDetails { get; set; }
    }
}