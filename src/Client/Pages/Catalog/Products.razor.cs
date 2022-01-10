using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
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

    // Fields for advanced search/filter
    protected bool CheckBox { get; set; } = true;

    // Fields for EditForm
    private List<BrandDto> _brands = new();

    protected override void OnInitialized()
    {
        Context = new(
            fields: new()
            {
                new(prod => prod.Id, L["Id"], "Id"),
                new(prod => prod.Name, L["Name"], "Name"),
                new(prod => prod.BrandName, L["Brand"], "Brand.Name"),
                new(prod => prod.Description, L["Description"], "Description"),
                new(prod => prod.Rate, L["Rate"], "Rate")
            },
            idFunc: prod => prod.Id,
            searchFunc: SearchFunc,
            createFunc: async prod => await ProductsClient.CreateAsync(prod.Adapt<CreateProductRequest>()),
            updateFunc: async (id, prod) => await ProductsClient.UpdateAsync(id, prod),
            deleteFunc: async id => await ProductsClient.DeleteAsync(id),
            editFormInitializedFunc: () => LoadBrandsAsync(),
            entityName: L["Product"],
            entityNamePlural: L["Products"],
            searchPermission: FSHPermissions.Products.Search,
            createPermission: FSHPermissions.Products.Register,
            updatePermission: FSHPermissions.Products.Update,
            deletePermission: FSHPermissions.Products.Remove);
    }

    private async Task<PaginatedResult<ProductDto>> SearchFunc(Components.EntityTable.PaginationFilter filter)
    {
        var productFilter = filter.Adapt<SearchProductsRequest>();

        // TODO: add advanced search and filter
        // filter.BrandId =
        // filter.MaximumRate =
        // filter.MinimumRate =

        var result = await ProductsClient.SearchAsync(productFilter);

        return result.Adapt<PaginatedResult<ProductDto>>();
    }

    // Functions for EditForm

    private async Task<IEnumerable<Guid>> SearchBrands(string value)
    {
        if (string.IsNullOrEmpty(value))
            return _brands.Select(x => x.Id);

        await LoadBrandsAsync(value);

        return _brands
            .Where(x => x.Name?.Contains(value, StringComparison.InvariantCultureIgnoreCase) ?? false)
            .Select(x => x.Id);
    }

    private async Task LoadBrandsAsync(string? searchKeyword = default)
    {
        string[] orderBy = { "id" };
        var filter = new SearchBrandsRequest { PageNumber = 0, PageSize = 10, OrderBy = orderBy };
        if (!string.IsNullOrEmpty(searchKeyword))
        {
            filter.Keyword = searchKeyword;
        }

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => BrandsClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfBrandDto response)
        {
            _brands = response.Data.ToList();
        }
    }
}