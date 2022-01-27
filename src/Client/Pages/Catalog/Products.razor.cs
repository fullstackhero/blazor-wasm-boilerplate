using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;

public partial class Products
{
    [Inject]
    protected IProductsClient ProductsClient { get; set; } = default!;
    [Inject]
    protected IBrandsClient BrandsClient { get; set; } = default!;

    protected EntityServerTableContext<ProductDto, Guid, UpdateProductRequest> Context { get; set; } = default!;

    private EntityTable<ProductDto, Guid, UpdateProductRequest> _table = default!;

    protected override void OnInitialized() =>
        Context = new(
            fields: new()
            {
                new(prod => prod.Id, L["Id"], "Id"),
                new(prod => prod.Name, L["Name"], "Name"),
                new(prod => prod.BrandName, L["Brand"], "Brand.Name"),
                new(prod => prod.Description, L["Description"], "Description"),
                new(prod => prod.Rate, L["Rate"], "Rate")
            },
            enableAdvancedSearch: true,
            idFunc: prod => prod.Id,
            searchFunc: SearchFunc,
            createFunc: async prod => await ProductsClient.CreateAsync(prod.Adapt<CreateProductRequest>()),
            updateFunc: async (id, prod) => await ProductsClient.UpdateAsync(id, prod),
            deleteFunc: async id => await ProductsClient.DeleteAsync(id),
            entityName: L["Product"],
            entityNamePlural: L["Products"],
            searchPermission: FSHPermissions.Products.Search,
            createPermission: FSHPermissions.Products.Register,
            updatePermission: FSHPermissions.Products.Update,
            deletePermission: FSHPermissions.Products.Remove);

    // Advanced Search

    private Guid _searchBrandId;
    private Guid SearchBrandId
    {
        get => _searchBrandId;
        set
        {
            _searchBrandId = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private decimal _searchMinimumRate;
    private decimal SearchMinimumRate
    {
        get => _searchMinimumRate;
        set
        {
            _searchMinimumRate = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private decimal _searchMaximumRate = 100;
    private decimal SearchMaximumRate
    {
        get => _searchMaximumRate;
        set
        {
            _searchMaximumRate = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private async Task<PaginationResponse<ProductDto>> SearchFunc(PaginationFilter filter)
    {
        var productFilter = filter.Adapt<SearchProductsRequest>();

        productFilter.BrandId = SearchBrandId == default ? null : SearchBrandId;
        productFilter.MinimumRate = SearchMinimumRate;
        productFilter.MaximumRate = SearchMaximumRate;

        var result = await ProductsClient.SearchAsync(productFilter);

        return result.Adapt<PaginationResponse<ProductDto>>();
    }
}