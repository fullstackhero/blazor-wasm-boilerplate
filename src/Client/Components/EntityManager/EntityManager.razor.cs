using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.EntityManager;

public partial class EntityManager<TEntity>
    where TEntity : class, new()
{
    [Parameter]
    [EditorRequired]
    public EntityManagerContext<TEntity> Context { get; set; } = default!;

    [Parameter]
    public bool Loading { get; set; }

    [Parameter]
    public RenderFragment? AdvancedSearchContent { get; set; }

    [Parameter]
    public RenderFragment<TEntity>? ExtraActions { get; set; }

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
    private IEnumerable<TEntity>? _entityList;
    private bool _dense;
    private bool _striped = true;
    private bool _bordered;
    private int _totalItems;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canSearch = (await AuthService.AuthorizeAsync(state.User, Context.SearchPermission)).Succeeded;
        _canCreate = (await AuthService.AuthorizeAsync(state.User, Context.CreatePermission)).Succeeded;
        _canUpdate = (await AuthService.AuthorizeAsync(state.User, Context.UpdatePermission)).Succeeded;
        _canDelete = (await AuthService.AuthorizeAsync(state.User, Context.DeletePermission)).Succeeded;

        await LoadDataAsync();
    }

    private bool HasActions => _canUpdate || _canDelete || Context.HasExtraActionsFunc is null || Context.HasExtraActionsFunc();

    // Client side paging/filtering
    private bool LocalSearch(TEntity entity)
    {
        if (string.IsNullOrWhiteSpace(_searchString) ||
            Context is not ClientEntityManagerContext<TEntity> clientContext ||
            clientContext.LocalSearchFunc is null)
        {
            return true;
        }

        return clientContext.LocalSearchFunc(_searchString, entity);
    }

    private async Task LoadDataAsync()
    {
        if (Loading || Context is not ClientEntityManagerContext<TEntity> clientContext)
        {
            return;
        }

        Loading = true;

        if (await ApiHelper.ExecuteCallGuardedAsync(() => clientContext.LoadDataFunc(), _snackBar) is ListResult<TEntity> result)
        {
            _entityList = result.Data;
        }

        Loading = false;
    }

    // Server Side paging/filtering
    private void OnSearch(string text)
    {
        if (Context is ServerEntityManagerContext<TEntity>)
        {
            _searchString = text;
            _table?.ReloadServerData();
        }
    }

    private Func<TableState, Task<TableData<TEntity>>>? ServerReloadFunc =>
        Context is ClientEntityManagerContext<TEntity>
            ? null
            : ServerReload;

    private async Task<TableData<TEntity>> ServerReload(TableState state)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        await LoadDataAsync(state);

        return new TableData<TEntity> { TotalItems = _totalItems, Items = _entityList };
    }

    private async Task LoadDataAsync(TableState state)
    {
        if (Loading || Context is not ServerEntityManagerContext<TEntity> serverContext)
        {
            return;
        }

        Loading = true;

        string[]? orderings = null;
        if (!string.IsNullOrEmpty(state.SortLabel))
        {
            orderings = state.SortDirection == SortDirection.None
                ? new[] { $"{state.SortLabel}" }
                : new[] { $"{state.SortLabel} {state.SortDirection}" };
        }

        var filter = new PaginationFilter
        {
            PageSize = state.PageSize,
            PageNumber = state.Page + 1,
            Keyword = _searchString,
            OrderBy = orderings ?? Array.Empty<string>()
        };

        var result = await serverContext.SearchFunc(filter);
        if (result.Succeeded)
        {
            _totalItems = result.TotalCount;
            _entityList = result.Data;
        }

        Loading = false;
    }

    private async Task InvokeModal(object? id = default)
    {
        var parameters = new DialogParameters()
        {
            { nameof(AddEditModal<TEntity>.Context), Context },
            { nameof(AddEditModal<TEntity>.EditFormContent), EditFormContent },
            { nameof(AddEditModal<TEntity>.IsCreate), id == default },
            { nameof(AddEditModal<TEntity>.Id), id }
        };

        if (id != default)
        {
            var entity = _entityList?.FirstOrDefault(c => Context.IdFunc(c) == id);
            if (entity is not null)
            {
                parameters.Add(nameof(AddEditModal<TEntity>.EntityModel), entity);
            }
        }

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<AddEditModal<TEntity>>(id == default ? L["Create"] : L["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await ResetAsync();
        }
    }

    private async Task Delete(object? id)
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
            await ApiHelper.ExecuteCallGuardedAsync(
                () => Context.DeleteFunc(id),
                _snackBar);

            await ResetAsync();
        }
    }

    private async Task ResetAsync()
    {
        if (Context is ClientEntityManagerContext<TEntity>)
        {
            await LoadDataAsync();
        }
        else
        {
            OnSearch(string.Empty);
        }
    }
}