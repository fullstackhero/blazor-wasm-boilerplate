using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
using FSH.WebApi.Shared.Authorization;
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
            entityName: L["Product"],
            entityNamePlural: L["Products"],
            entityResource: FSHResource.Products,
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
            searchFunc: async filter =>
            {
                var productFilter = filter.Adapt<SearchProductsRequest>();

                productFilter.BrandId = SearchBrandId == default ? null : SearchBrandId;
                productFilter.MinimumRate = SearchMinimumRate;
                productFilter.MaximumRate = SearchMaximumRate;

                var result = await ProductsClient.SearchAsync(productFilter);
                return result.Adapt<PaginationResponse<ProductDto>>();
            },
            createFunc: async prod =>
            {
                if (!string.IsNullOrEmpty(prod.ImageInBytes))
                {
                    prod.Image = new FileUploadRequest() { Data = prod.ImageInBytes, Extension = prod.ImageExtension ?? string.Empty, Name = $"{prod.Name}_{Guid.NewGuid():N}" };
                }

                await ProductsClient.CreateAsync(prod.Adapt<CreateProductRequest>());
                prod.ImageInBytes = string.Empty;
            },
            updateFunc: async (id, prod) =>
            {
                if (!string.IsNullOrEmpty(prod.ImageInBytes))
                {
                    prod.DeleteCurrentImage = true;
                    prod.Image = new FileUploadRequest() { Data = prod.ImageInBytes, Extension = prod.ImageExtension ?? string.Empty, Name = $"{prod.Name}_{Guid.NewGuid():N}" };
                }

                await ProductsClient.UpdateAsync(id, prod.Adapt<UpdateProductRequest>());
                prod.ImageInBytes = string.Empty;
            },
            deleteFunc: async id => await ProductsClient.DeleteAsync(id));

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

    // TODO : Make this as a shared service or something? Since it's used by Profile Component also for now, and literally any other component that will have image upload.
    // The new service should ideally return $"data:{ApplicationConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}"
    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        if (e.File != null)
        {
            string? extension = Path.GetExtension(e.File.Name);
            if (!ApplicationConstants.SupportedImageFormats.Contains(extension.ToLower()))
            {
                Snackbar.Add("Image Format Not Supported.", Severity.Error);
                return;
            }

            Context.AddEditModal.RequestModel.ImageExtension = extension;
            var imageFile = await e.File.RequestImageFileAsync(ApplicationConstants.StandardImageFormat, ApplicationConstants.MaxImageWidth, ApplicationConstants.MaxImageHeight);
            byte[]? buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream(ApplicationConstants.MaxAllowedSize).ReadAsync(buffer);
            Context.AddEditModal.RequestModel.ImageInBytes = $"data:{ApplicationConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}";
            Context.AddEditModal.ForceRender();
        }
    }

    public void ClearImageInBytes()
    {
        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
        Context.AddEditModal.ForceRender();
    }

    public void SetDeleteCurrentImageFlag()
    {
        Context.AddEditModal.RequestModel.ImageInBytes = string.Empty;
        Context.AddEditModal.RequestModel.ImagePath = string.Empty;
        Context.AddEditModal.RequestModel.DeleteCurrentImage = true;
        Context.AddEditModal.ForceRender();
    }
}

public class ProductViewModel : UpdateProductRequest
{
    public string? ImagePath { get; set; }
    public string? ImageInBytes { get; set; }
    public string? ImageExtension { get; set; }
}