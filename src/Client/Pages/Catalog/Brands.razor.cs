using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;

public partial class Brands
{
    [Inject]
    private IBrandsClient _brandsClient { get; set; } = default!;

    private IEnumerable<BrandDto>? _pagedData;
    private TableState? _state;
    private MudTable<BrandDto>? _table;
    private string _searchString = string.Empty;
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;
    private int _currentPage;
    public bool checkBox { get; set; } = true;

    // private ClaimsPrincipal _currentUser;
    private bool _canCreateBrands;
    private bool _canEditBrands;
    private bool _canDeleteBrands;
    private bool _canSearchBrands;
    private bool _loading;
    private int _totalItems;

    protected override Task OnInitializedAsync()
    {
        // _currentUser = _stateProvider.AuthenticationStateUser;
        _canCreateBrands = true; // (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Create)).Succeeded;
        _canEditBrands = true; // (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Edit)).Succeeded;
        _canDeleteBrands = true; // (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Delete)).Succeeded;
        _canSearchBrands = true; // (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Search)).Succeeded;
        return Task.CompletedTask;
    }

    private async Task<TableData<BrandDto>> ServerReload(TableState state)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        _state = state;

        await GetBrandsAsync();

        return new TableData<BrandDto> { TotalItems = _totalItems, Items = _pagedData };
    }

    private async Task GetBrandsAsync()
    {
        if (_loading)
        {
            return;
        }

        _loading = true;

        string[] orderings = string.IsNullOrEmpty(_state?.SortLabel)
            ? Array.Empty<string>()
            : _state.SortDirection != SortDirection.None
                ? new[] { $"{_state.SortLabel} {_state.SortDirection}" }
                : new[] { $"{_state.SortLabel}" };

        var filter = new BrandListFilter
        {
            PageSize = _state == null ? 10 : _state.PageSize,
            PageNumber = (_state == null ? 0 : _state.Page) + 1,
            Keyword = _searchString,
            OrderBy = orderings
        };

        var response = await _brandsClient.SearchAsync(filter);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            _pagedData = response.Data;
        }

        _loading = false;
    }

    private async Task Delete(Guid id)
    {
        string deleteContent = _localizer["Delete Content"];
        var parameters = new DialogParameters
            {
                { nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id) }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var response = await _brandsClient.DeleteAsync(id);
            if (response.Succeeded)
            {
                if (response.Messages?.Count > 0)
                    _snackBar.Add(response.Messages.First(), Severity.Success);
                else
                    _snackBar.Add(_localizer["Success"], Severity.Success);
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

            await Reset();
        }
    }

    private async Task InvokeModal(Guid id = default)
    {
        var parameters = new DialogParameters
            {
                { nameof(AddEditBrandModal.IsCreate), id == default },
                { nameof(AddEditBrandModal.Id), id }
            };
        if (id != default)
        {
            var brand = _pagedData?.FirstOrDefault(c => c.Id == id);
            if (brand != null)
            {
                parameters.Add(nameof(AddEditBrandModal.UpdateBrandRequest), new UpdateBrandRequest
                {
                    Name = brand.Name,
                    Description = brand.Description,
                });
            }
        }

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<AddEditBrandModal>(id == default ? _localizer["Create"] : _localizer["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await Reset();
        }
    }

    private async Task Reset()
    {
        await GetBrandsAsync();
        OnSearch(string.Empty);
        StateHasChanged();
    }

    private bool Search(BrandDto brand)
    {
        if (string.IsNullOrWhiteSpace(_searchString)) return true;
        if (brand.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return brand.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true;
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table?.ReloadServerData();
    }
}