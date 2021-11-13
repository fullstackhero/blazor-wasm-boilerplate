using FSH.BlazorWebAssembly.Shared.Catalog;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog
{
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
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            //await LoadImageAsync();
            //await LoadBrandsAsync();
        }

        //private async Task LoadBrandsAsync()
        //{
        //    var data = await _brandService.GetAllAsync();
        //    if (data.Succeeded)
        //    {
        //        _brands = data.Data;
        //    }
        //}

        //private async Task LoadImageAsync()
        //{
        //    var data = await _productService.GetProductImageAsync(UpdateProductRequest.Id);
        //    if (data.Succeeded)
        //    {
        //        var imageData = data.Data;
        //        if (!string.IsNullOrEmpty(imageData))
        //        {
        //            UpdateProductRequest.ImageDataURL = imageData;
        //        }
        //    }
        //}

        private void DeleteAsync()
        {
            //UpdateProductRequest.ImageDataURL = null;
            //UpdateProductRequest.UploadRequest = new UploadRequest();
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
                //UpdateProductRequest.UploadRequest = new UploadRequest { Data = buffer, UploadType = Application.Enums.UploadType.Product, Extension = extension };
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
}