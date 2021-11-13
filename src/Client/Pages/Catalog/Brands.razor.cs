using FSH.BlazorWebAssembly.Shared.Catalog;
using MudBlazor;
using System.Security.Claims;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;
public partial class Brands
{

    private List<BrandDto> _brandList = new();
    private BrandDto _brand = new();
    private string _searchString = "";
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;

    private ClaimsPrincipal _currentUser;
    private bool _canCreateBrands;
    private bool _canEditBrands;
    private bool _canDeleteBrands;
    private bool _canSearchBrands;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        _currentUser = _stateProvider.AuthenticationStateUser; 
        _canCreateBrands = true;// (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Create)).Succeeded;
        _canEditBrands = true;// (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Edit)).Succeeded;
        _canDeleteBrands = true;//(await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Delete)).Succeeded;
        _canSearchBrands = true;//(await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Brands.Search)).Succeeded;

        await GetBrandsAsync();
        _loaded = true;

    }

    private async Task GetBrandsAsync()
    {
        string[] orderBy = { "id" };
        BrandListFilter filter = new() { PageNumber = 1, PageSize = 10, OrderBy = orderBy };
        var response = await _brandService.SearchBrandAsync(filter);
        if (response.Succeeded)
        {
            _brandList = response.Data.ToList();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
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
            _brand = _brandList?.FirstOrDefault(c => c.Id == id);
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
        _brand = new BrandDto();
        await GetBrandsAsync();
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
}