using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
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
    private MudDialogInstance MudDialog { get; set; } = default!;

    [Inject]
    public IBrandsClient _brandsClient { get; set; } = default!;

    [Inject]
    public IProductsClient _productsClient { get; set; } = default!;

    private List<BrandDto> _brands = new();

    public void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task SaveAsync()
    {
        ResultOfGuid response;
        if (IsCreate)
        {
            CreateProductRequest createBrandRequest = new() { Name = UpdateProductRequest.Name, Description = UpdateProductRequest.Description, BrandId = UpdateProductRequest.BrandId, Rate = UpdateProductRequest.Rate };
            response = await _productsClient.CreateAsync(createBrandRequest);
        }
        else
        {
            response = await _productsClient.UpdateAsync(Id, UpdateProductRequest);
        }

        if (response.Succeeded)
        {
            if (response.Messages?.Count > 0)
                _snackBar.Add(response.Messages.First(), Severity.Success);
            else
                _snackBar.Add(_localizer["Success"], Severity.Success);
            MudDialog.Close();
        }
        else
        {
            if (response.Messages?.Count > 0)
            {
                foreach (string message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }

    protected override async Task OnInitializedAsync() => await LoadDataAsync();

    private async Task LoadDataAsync()
    {
        await LoadBrandsAsync();
    }

    private async Task LoadBrandsAsync(string? searchKeyword = default)
    {
        string[] orderBy = { "id" };
        BrandListFilter filter = new() { PageNumber = 0, PageSize = 10, OrderBy = orderBy };
        if (string.IsNullOrEmpty(searchKeyword)) filter.Keyword = searchKeyword;
        var response = await _brandsClient.SearchAsync(filter);
        if (response.Succeeded && response.Data is not null)
        {
            _brands = response.Data.ToList();
        }
    }

    public async Task<IEnumerable<Guid>> SearchBrands(string value)
    {
        if (string.IsNullOrEmpty(value))
            return _brands.Select(x => x.Id);

        await LoadBrandsAsync(value);
        return _brands.Where(x => x.Name?.Contains(value, StringComparison.InvariantCultureIgnoreCase) ?? false)
            .Select(x => x.Id);
    }
}