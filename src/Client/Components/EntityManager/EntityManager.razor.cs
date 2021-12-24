using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.EntityManager;

public partial class EntityManager<TEntity, TFilter>
    where TEntity : class, new()
    where TFilter : PaginationFilter, new()
{
    [Parameter]
    [EditorRequired]
    public EntityManagerContext<TEntity, TFilter> Context { get; set; } = default!;

    [Parameter]
    public RenderFragment? AdvancedSearchContent { get; set; }

    [Parameter]
    [EditorRequired]
    public RenderFragment<TEntity> EditFormContent { get; set; } = default!;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    private bool _canSearch;
    private bool _canCreate;
    private bool _canUpdate;
    private bool _canDelete;

    private string? _searchString;

    private MudTable<TEntity>? _table;
    private IEnumerable<TEntity>? _pagedData;
    private bool _dense;
    private bool _striped = true;
    private bool _bordered;
    private bool _loading;
    private int _totalItems;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, Context.SearchPermission)).Succeeded;
        _canCreate = (await AuthService.AuthorizeAsync(state.User, Context.CreatePermission)).Succeeded;
        _canUpdate = (await AuthService.AuthorizeAsync(state.User, Context.UpdatePermission)).Succeeded;
        _canDelete = (await AuthService.AuthorizeAsync(state.User, Context.DeletePermission)).Succeeded;
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table?.ReloadServerData();
    }

    private async Task<TableData<TEntity>> ServerReload(TableState state)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        await LoadDataAsync(state);

        return new TableData<TEntity> { TotalItems = _totalItems, Items = _pagedData };
    }

    private async Task LoadDataAsync(TableState state)
    {
        if (!_loading)
        {
            _loading = true;

            string[]? orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection == SortDirection.None
                    ? new[] { $"{state.SortLabel}" }
                    : new[] { $"{state.SortLabel} {state.SortDirection}" };
            }

            var filter = new TFilter
            {
                PageSize = state.PageSize,
                PageNumber = state.Page + 1,
                Keyword = _searchString,
                OrderBy = orderings ?? Array.Empty<string>()
            };

            var result = await Context.SearchFunc(filter);
            if (result.Succeeded)
            {
                _totalItems = result.TotalCount;
                _pagedData = result.Data;
            }

            _loading = false;
        }
    }

    private async Task InvokeModal(Guid id = default)
    {
        var parameters = new DialogParameters()
        {
            { nameof(AddEditModal<TEntity, TFilter>.Context), Context },
            { nameof(AddEditModal<TEntity, TFilter>.EditFormContent), EditFormContent },
            { nameof(AddEditModal<TEntity, TFilter>.IsCreate), id == default },
            { nameof(AddEditModal<TEntity, TFilter>.Id), id }
        };

        if (id != default)
        {
            var entity = _pagedData?.FirstOrDefault(c => Context.IdFunc(c) == id);
            if (entity is not null)
            {
                parameters.Add(nameof(AddEditModal<TEntity, TFilter>.EntityModel), entity);
            }
        }

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<AddEditModal<TEntity, TFilter>>(id == default ? L["Create"] : L["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            OnSearch(string.Empty);
        }
    }

    private async Task Delete(Guid id)
    {
        string deleteContent = L["You're sure you want to delete {0} with id '{1}'?"];
        var parameters = new DialogParameters
        {
            { nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, Context.EntityName, id) }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(L["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var response = await Context.DeleteFunc(id);
            if (response.Succeeded)
            {
                if (response.Messages?.FirstOrDefault() is string message)
                {
                    _snackBar.Add(message, Severity.Success);
                }
            }
            else if (response.Messages is not null)
            {
                foreach (string message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            OnSearch(string.Empty);
        }
    }
}