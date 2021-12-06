using FSH.BlazorWebAssembly.Shared.Catalog;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;

public partial class AddEditProductModal
{
    [Parameter]
    public UpdateProductRequest UpdateProductRequest { get; set; } = new();

    [Parameter]
    public bool IsCreate { get; set; }

    [Parameter]
    public Guid Id { get; set; }

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; }

    private List<BrandDto> _brands = new();

    public void Cancel()
    {
        MudDialog?.Cancel();
    }

    private async Task SaveAsync()
    {
        IResult<Guid> response;
        if (IsCreate)
        {
            CreateProductRequest createBrandRequest = new() { Name = UpdateProductRequest.Name, Description = UpdateProductRequest.Description, BrandId = UpdateProductRequest.BrandId, Rate = UpdateProductRequest.Rate };
            response = await _productService.CreateAsync(createBrandRequest);
        }
        else
        {
            response = await _productService.UpdateAsync(this.UpdateProductRequest, Id);
        }

        if (response.Succeeded)
        {
            if (response.Messages.Count > 0)
                _snackBar.Add(response.Messages[0], Severity.Success);
            else
                _snackBar.Add(_localizer["Success"], Severity.Success);
            MudDialog?.Close();
        }
        else
        {
            if (response.Messages.Count > 0)
            {
                foreach (string message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            else if (!string.IsNullOrEmpty(response.Exception))
            {
                _snackBar.Add(response.Exception, Severity.Error);
            }
        }
    }

    protected override async Task OnInitializedAsync() => await LoadDataAsync();

    private async Task LoadDataAsync()
    {
        await LoadBrandsAsync();
    }

    private async Task LoadBrandsAsync(string searchKeyword = default)
    {
        string[] orderBy = { "id" };
        BrandListFilter filter = new() { PageNumber = 0, PageSize = 0, OrderBy = orderBy };
        if (string.IsNullOrEmpty(searchKeyword)) filter.Keyword = searchKeyword;
        var response = await _brandService.SearchBrandAsync(filter);
        if (response.Succeeded)
        {
            _brands = response.Data.ToList();
        }
    }

    public async Task<IEnumerable<Guid>> SearchBrands(string value)
    {
        if (string.IsNullOrEmpty(value))
            return _brands.Select(x => x.Id);

        await LoadBrandsAsync(value);
        return _brands.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Id);
    }
}