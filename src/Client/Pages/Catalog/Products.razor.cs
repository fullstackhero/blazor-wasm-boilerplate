using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
using FSH.BlazorWebAssembly.Client.Shared;
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

    protected override async Task OnInitializedAsync()
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
                    enableAdvancedSearch: true,
                    idFunc: prod => prod.Id,
                    searchFunc: SearchFunc,
                    createFunc: async prod =>
                    {
                        if (!string.IsNullOrEmpty(Context.AddEditModal.RequestModel.ImageInBytes))
                        {
                            prod.Image = new FileUploadRequest() { Data = Context.AddEditModal.RequestModel.ImageInBytes, Extension = Context.AddEditModal.RequestModel.ImageExtension, Name = $"{prod.Name}-{Guid.NewGuid():N}" };
                        }

                        await ProductsClient.CreateAsync(prod.Adapt<CreateProductRequest>());
                        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
                    },
                    updateFunc: async (id, prod) =>
                    {
                        if (!string.IsNullOrEmpty(Context.AddEditModal.RequestModel.ImageInBytes))
                        {
                            prod.Image = new FileUploadRequest() { Data = Context.AddEditModal.RequestModel.ImageInBytes, Extension = Context.AddEditModal.RequestModel.ImageExtension, Name = $"{prod.Name}-{Guid.NewGuid():N}" };
                        }

                        await ProductsClient.UpdateAsync(id, prod);
                        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
                    },
                    deleteFunc: async id => await ProductsClient.DeleteAsync(id),
                    entityName: L["Product"],
                    entityNamePlural: L["Products"],
                    searchPermission: FSHPermissions.Products.Search,
                    createPermission: FSHPermissions.Products.Register,
                    updatePermission: FSHPermissions.Products.Update,
                    deletePermission: FSHPermissions.Products.Remove);
        await LoadBrandsAsync();
    }

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

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        _file = e.File;
        if (_file != null)
        {
            Context.AddEditModal.RequestModel.ImageExtension = Path.GetExtension(_file.Name);
            if (!ApplicationConstants.SupportedImageFormats.Contains(Context.AddEditModal.RequestModel.ImageExtension.ToLower()))
            {
                Snackbar.Add("Image Format Not Supported.", Severity.Error);
                return;
            }

            var imageFile = await e.File.RequestImageFileAsync(ApplicationConstants.StandardImageFormat, ApplicationConstants.MaxImageWidth, ApplicationConstants.MaxImageHeight);
            byte[]? buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            Context.AddEditModal.RequestModel.ImageInBytes = $"data:{ApplicationConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}";
            Context.AddEditModal?.ForceRender();
        }
    }

    public List<BrandDto> _brands { get; set; } = new();

    private async Task LoadBrandsAsync()
    {
        var filter = new SearchBrandsRequest
        {
            PageSize = 100
        };
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => BrandsClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfBrandDto response)
        {
            _brands = response.Data.ToList();
        }
    }

    private async Task<IEnumerable<Guid>> SearchBrands(string value)
    {
        var filter = new SearchBrandsRequest
        {
            PageSize = 10,
            AdvancedSearch = new() { Fields = new[] { "name" }, Keyword = value }
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => BrandsClient.SearchAsync(filter), Snackbar)
            is PaginationResponseOfBrandDto response)
        {
            _brands = response.Data.ToList();
        }

        return _brands.Select(x => x.Id);
    }

    private string GetBrandName(Guid id) =>
        _brands.Find(b => b.Id == id)?.Name ?? string.Empty;

    public void ClearImageInBytes()
    {
        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
        Context.AddEditModal?.ForceRender();
    }
}

public class ProductViewModel : UpdateProductRequest
{
    public string? ImagePath { get; set; }

    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
}