using FSH.BlazorWebAssembly.Client.Shared;
using FSH.BlazorWebAssembly.Shared.Catalog;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Security.Claims;

namespace FSH.BlazorWebAssembly.Client.Pages.Catalog;
public partial class Products
{
    [CascadingParameter]
    public Error Error { get; set; }

    private IEnumerable<ProductDto> _pagedData;
    private MudTable<ProductDto> _table;

    private int _totalItems;
    private int _currentPage;
    private string _searchString = "";
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;
    private bool _loading = true;

    private ClaimsPrincipal _currentUser;
    private bool _canCreateProducts;
    private bool _canEditProducts;
    private bool _canDeleteProducts;
    private bool _canSearchProducts;

    public bool Label_CheckBox1 { get; set; } = true;
    protected override async Task OnInitializedAsync()
    {
        _currentUser = _stateProvider.AuthenticationStateUser;
        _canCreateProducts = true;// (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Products.Create)).Succeeded;
        _canEditProducts = true;//(await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Products.Edit)).Succeeded;
        _canDeleteProducts = true;//(await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Products.Delete)).Succeeded;
        _canSearchProducts = true;//(await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Products.Search)).Succeeded;
    }

    private async Task<TableData<ProductDto>> ServerReload(TableState state)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }
        await LoadData(state.Page, state.PageSize, state);
        return new TableData<ProductDto> { TotalItems = _totalItems, Items = _pagedData };
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state)
    {
        _loading = true;
        try
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var request = new ProductListFilter { PageSize = pageSize, PageNumber = pageNumber + 1, Keyword = _searchString, OrderBy = orderings };
            var response = await _productService.GetProductsAsync(request);
            if (response.Succeeded)
            {
                _totalItems = response.TotalCount;
                _currentPage = response.CurrentPage;
                _pagedData = response.Data;
            }
            else
            {
                Error.ProcessError(response.Messages);
            }
        }
        catch (Exception ex)
        {
            Error.ProcessError(ex);
        }
        _loading = false;
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private async Task InvokeModal(Guid id = new())
    {
        var parameters = new DialogParameters()
            {
                { nameof(AddEditProductModal.IsCreate), id == new Guid() },
                { nameof(AddEditProductModal.Id), id }
            };
        if (id != new Guid())
        {
            var product = _pagedData.FirstOrDefault(c => c.Id == id);
            if (product != null)
            {
                parameters.Add(nameof(AddEditProductModal.UpdateProductRequest), new UpdateProductRequest
                {
                    Name = product.Name,
                    Description = product.Description,
                    Rate = product.Rate,
                    BrandId = product.BrandId
                });
            }
        }
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<AddEditProductModal>(id == new Guid() ? _localizer["Create"] : _localizer["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            OnSearch("");
        }
    }

    private async Task Delete(Guid id)
    {
        string deleteContent = _localizer["Delete Content"];
        var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id)}
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var response = await _productService.DeleteAsync(id);
            if (response.Succeeded)
            {
                OnSearch("");
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                OnSearch("");
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}