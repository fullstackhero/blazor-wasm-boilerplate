using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;

// Avoiding this for now. We Would probably have to create a seperate Razor Component instead of just extending the class.
// Something like a BrandAutoComplete.razor which internally makes use of MudAutoComplete using @inherits setter.
public class BrandAutocomplete : MudAutocomplete<Guid>
{
    [Inject]
    private IStringLocalizer<BrandAutocomplete> L { get; set; } = default!;
    [Inject]
    private IBrandsClient BrandsClient { get; set; } = default!;
    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private List<BrandDto> _brands = new();

    // supply default parameters, but leave the possibility to override them
    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = L["Brand"];
        Variant = Variant.Filled;
        Dense = true;
        Margin = Margin.Dense;
        ResetValueOnEmptyText = true;
        SearchFunc = SearchBrands;
        ToStringFunc = GetBrandName;
        Clearable = true;
        return base.SetParametersAsync(parameters);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (_value != default)
            {
                if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => BrandsClient.GetAsync(_value), Snackbar) is { } brand)
                {
                    _brands.Add(brand);
                    ForceRender(true);
                }
            }
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
}