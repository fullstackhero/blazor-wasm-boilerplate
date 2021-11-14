using FSH.BlazorWebAssembly.Shared.Catalog;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;
public partial class AddEditProductModal
{
    [Parameter] public UpdateProductRequest UpdateProductRequest { get; set; } = new();
    [Parameter] public bool IsCreate { get; set; }
    [Parameter] public Guid Id { get; set; }
    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    private List<BrandDto> _brands = new();

    public void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task SaveAsync()
    {
        IResult<Guid> response;
        if (IsCreate)
        {
            CreateProductRequest CreateBrandRequest = new() { Name = UpdateProductRequest.Name, Description = UpdateProductRequest.Description, BrandId = UpdateProductRequest.BrandId, Rate = UpdateProductRequest.Rate };
            response = await _productService.CreateAsync(CreateBrandRequest);
        }
        else
        {
            response = await _productService.UpdateAsync(this.UpdateProductRequest, Id);
        }
        if (response.Succeeded)
        {
            if (response.Messages.Any())
                _snackBar.Add(response.Messages[0], Severity.Success);
            else
                _snackBar.Add(_localizer["Success"], Severity.Success);
            MudDialog.Close();
        }
        else
        {
            if (response.Messages.Any()) foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            else if (!string.IsNullOrEmpty(response.Exception))
                _snackBar.Add(response.Exception, Severity.Error);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        //await LoadImageAsync();
        await LoadBrandsAsync();
    }

    private async Task LoadBrandsAsync()
    {
        string[] orderBy = { "id" };
        BrandListFilter filter = new() { PageNumber = 0, PageSize = 0, OrderBy = orderBy };
        var response = await _brandService.SearchBrandAsync(filter);
        if (response.Succeeded)
        {
            _brands = response.Data.ToList();
        }
    }

    private async Task LoadImageAsync()
    {
        var data = await _productService.GetProductImageAsync(UpdateProductRequest.BrandId);
        if (data.Succeeded)
        {
            var imageData = data.Data;
            if (!string.IsNullOrEmpty(imageData))
            {
                //UpdateProductRequest.ImageDataURL = imageData;
            }
        }
    }

    private void DeleteAsync()
    {
        //UpdateProductRequest.ImageDataURL = null;
        UpdateProductRequest.Image = new FileUploadRequest();
    }

    private IBrowserFile _file;

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        _file = e.File;
        if (_file != null)
        {
            var extension = Path.GetExtension(_file.Name);
            var format = "image/png";
            var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
            var buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            //UpdateProductRequest.ImageDataURL = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
            UpdateProductRequest.Image = new FileUploadRequest { Data = Convert.ToString(buffer), Extension = extension };
        }
    }

    private async Task<IEnumerable<Guid>> SearchBrands(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
            return _brands.Select(x => x.Id);

        return _brands.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Id);
    }
}