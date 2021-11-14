using FSH.BlazorWebAssembly.Shared.Catalog;
using MudBlazor;
using System.Security.Claims;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;
public partial class Brands
{
    private IEnumerable<BrandDto> _pagedData;
    private TableState _state;
    private MudTable<BrandDto> _table;
    private string _searchString = "";
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;
    private int _currentPage;
    public bool Label_CheckBox1 { get; set; } = true;
    private ClaimsPrincipal _currentUser;
    private bool _canCreateBrands;
    private bool _canEditBrands;
    private bool _canDeleteBrands;
    private bool _canSearchBrands;
    private bool _loading = true;
    private int _totalItems;
    protected override async Task OnInitializedAsync()
    {
        _currentUser = _stateProvider.AuthenticationStateUser;
        _canCreateBrands = true;// (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Create)).Succeeded;
        _canEditBrands = true;// (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Edit)).Succeeded;
        _canDeleteBrands = true;//(await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Delete)).Succeeded;
        _canSearchBrands = true;//(await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Search)).Succeeded;

        await GetBrandsAsync();
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
        _loading = true;
        await Task.Delay(200);//test
        string[] orderings = null;
        if (!string.IsNullOrEmpty(_state.SortLabel))
        {
            orderings = _state.SortDirection != SortDirection.None ? new[] { $"{_state.SortLabel} {_state.SortDirection}" } : new[] { $"{_state.SortLabel}" };
        }
        BrandListFilter filter = new() { PageSize = _state.PageSize, PageNumber = _state.Page + 1, Keyword = _searchString, OrderBy = orderings };
        var response = await _brandService.SearchBrandAsync(filter);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            _pagedData = response.Data.ToList();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
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
            var response = await _brandService.DeleteAsync(id);
            if (response.Succeeded)
            {
                await Reset();
                if (response.Messages.Any())
                    _snackBar.Add(response.Messages[0], Severity.Success);
                else
                    _snackBar.Add(_localizer["Success"], Severity.Success);
            }
            else
            {
                await Reset();
                if (response.Messages.Any()) foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                else if (!string.IsNullOrEmpty(response.Exception))
                    _snackBar.Add(response.Exception, Severity.Error);
            }
        }
    }
    private async Task InvokeModal(Guid id = new())
    {
        var parameters = new DialogParameters
            {
                { nameof(AddEditBrandModal.IsCreate), id == new Guid() },
                { nameof(AddEditBrandModal.Id), id }
            };
        if (id != new Guid())
        {
            var _brand = _pagedData?.FirstOrDefault(c => c.Id == id);
            if (_brand != null)
            {
                parameters.Add(nameof(AddEditBrandModal.UpdateBrandRequest), new UpdateBrandRequest
                {
                    Name = _brand.Name,
                    Description = _brand.Description,
                });

            }
        }
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<AddEditBrandModal>(id == new Guid() ? _localizer["Create"] : _localizer["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await Reset();
        }
    }
    private async Task Reset()
    {
        await GetBrandsAsync();
        OnSearch("");
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
        _table.ReloadServerData();
    }
}