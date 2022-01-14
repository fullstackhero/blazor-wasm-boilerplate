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
    private string? _searchString;
    protected EntityClientTableContext<TenantDetail, Guid, CreateTenantRequest> Context { get; set; } = default!;
    private List<TenantDetail> _tenants = new();

    protected override void OnInitialized() =>
        Context = new(
            fields: new()
            {
                new(tenant => tenant.Id, L["Id"]),
                new(tenant => tenant.Key, L["Key"]),
                new(tenant => tenant.Name, L["Name"]),
                new(tenant => tenant.AdminEmail, L["Admin Email"])
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

    private void ShowBtnPress(Guid id)
    {
        var tenant = _tenants.First(f => f.Id == id);
        tenant.ShowDetails = !tenant.ShowDetails;
        foreach (var otherTenants in _tenants.Except(new[] { tenant }))
        {
            otherTenants.ShowDetails = false;
        }
    }

    public class TenantDetail : TenantDto
    {
        public bool ShowDetails { get; set; }
    }
}