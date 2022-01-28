using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;

public partial class Products
{
    [Inject]
    protected IProductsClient ProductsClient { get; set; } = default!;
    [Inject]
    protected IBrandsClient BrandsClient { get; set; } = default!;

    protected EntityServerTableContext<ProductDto, Guid, ProductViewModel> Context { get; set; } = default!;

    private EntityTable<ProductDto, Guid, ProductViewModel> _table = default!;

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
            createFunc: async prod =>
            {
                if (!string.IsNullOrEmpty(ProductImage))
                {
                    prod.Image = new FileUploadRequest() { Data = ProductImage, Extension = ProductImageExtension, Name = $"{prod.Name}-{Guid.NewGuid():N}" };
                }

                await ProductsClient.CreateAsync(prod.Adapt<CreateProductRequest>());
                ProductImage = string.Empty;
            },
            updateFunc: async (id, prod) =>
            {
                if(!string.IsNullOrEmpty(ProductImage))
                {
                    prod.Image = new FileUploadRequest() { Data = ProductImage, Extension = ProductImageExtension, Name = $"{prod.Name}-{Guid.NewGuid():N}" };
                }

                await ProductsClient.UpdateAsync(id, prod);
                ProductImage = string.Empty;
            },
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

    private decimal _searchMaximumRate = 9999;
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

    private IBrowserFile? _file;

    public string? ProductImage { get; set; }
    public string? ProductImageExtension { get; set; }
    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        _file = e.File;
        if (_file != null)
        {
            ProductImageExtension = Path.GetExtension(_file.Name);
            if (!ApplicationConstants.SupportedImageFormats.Contains(ProductImageExtension.ToLower()))
            {
                Snackbar.Add("Image Format Not Supported.", Severity.Error);
                return;
            }

            string? format = "image/png";
            var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
            byte[]? buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            ProductImage = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
        }
    }
}

public class ProductViewModel : UpdateProductRequest
{
    public string? ImagePath { get; set; }
}