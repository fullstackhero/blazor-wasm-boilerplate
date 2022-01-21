using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;

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
        var dict = parameters.ToDictionary();
        if (!dict.ContainsKey(nameof(Label)))
            Label = L["Brand"];
        if (!dict.ContainsKey(nameof(Variant)))
            Variant = Variant.Filled;
        if (!dict.ContainsKey(nameof(Dense)))
            Dense = true;
        if (!dict.ContainsKey(nameof(Margin)))
            Margin = Margin.Dense;
        if (!dict.ContainsKey(nameof(ResetValueOnEmptyText)))
            ResetValueOnEmptyText = true;
        if (!dict.ContainsKey(nameof(SearchFunc)))
            SearchFunc = SearchBrands;
        if (!dict.ContainsKey(nameof(ToStringFunc)))
            ToStringFunc = GetBrandName;
        if (!dict.ContainsKey(nameof(Clearable)))
            Clearable = true;
        return base.SetParametersAsync(parameters);
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