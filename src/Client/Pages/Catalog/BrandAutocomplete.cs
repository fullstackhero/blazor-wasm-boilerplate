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

    // when the value parameter is set, we have to load that one brand to be able to show the name
    // we can't do that in OnInitialized because of a strange bug (https://github.com/MudBlazor/MudBlazor/issues/3818)
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender &&
            _value != default &&
            await ApiHelper.ExecuteCallGuardedAsync(
                () => BrandsClient.GetAsync(_value), Snackbar) is { } brand)
        {
            _brands.Add(brand);
            ForceRender(true);
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