using FSH.BlazorWebAssembly.Client.Shared;
using FSH.BlazorWebAssembly.Client.Shared.Dialogs;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

public partial class EntityTable<TEntity, TId, TRequest>
    where TRequest : new()
{
    [Parameter]
    [EditorRequired]
    public EntityTableContext<TEntity, TId, TRequest> Context { get; set; } = default!;

    [Parameter]
    public bool Dense { get; set; }
    [Parameter]
    public bool Striped { get; set; } = true;
    [Parameter]
    public bool Bordered { get; set; }
    [Parameter]
    public bool Loading { get; set; }

    [Parameter]
    public string? SearchString { get; set; }
    [Parameter]
    public EventCallback<string> SearchStringChanged { get; set; }

    [Parameter]
    public RenderFragment? AdvancedSearchContent { get; set; }

    [Parameter]
    public RenderFragment<TEntity>? ActionsContent { get; set; }
    [Parameter]
    public RenderFragment<TEntity>? ExtraActions { get; set; }
    [Parameter]
    public RenderFragment<TEntity>? ChildRowContent { get; set; }

    [Parameter]
    public RenderFragment<TRequest>? EditFormContent { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    private bool _canSearch;
    private bool _canCreate;
    private bool _canUpdate;
    private bool _canDelete;

    private bool _advancedSearchExpanded;

    private MudTable<TEntity>? _table;
    private IEnumerable<TEntity>? _entityList;
    private int _totalItems;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canSearch = await CanDoPermission(Context.SearchPermission, state);
        _canCreate = await CanDoPermission(Context.CreatePermission, state);
        _canUpdate = await CanDoPermission(Context.UpdatePermission, state);
        _canDelete = await CanDoPermission(Context.DeletePermission, state);

        await LoadDataAsync();
    }

    private async Task<bool> CanDoPermission(string? permission, AuthenticationState state) =>
        !string.IsNullOrWhiteSpace(permission) &&
            ((bool.TryParse(permission, out bool can) && can) || // check if permmission equals "True", then it's allowed
            (await AuthService.AuthorizeAsync(state.User, permission)).Succeeded);

    private bool HasActions => _canUpdate || _canDelete || Context.HasExtraActionsFunc is null || Context.HasExtraActionsFunc();
    private bool CanUpdateEntity(TEntity entity) => _canUpdate && (Context.CanUpdateEntityFunc is null || Context.CanUpdateEntityFunc(entity));
    private bool CanDeleteEntity(TEntity entity) => _canDelete && (Context.CanDeleteEntityFunc is null || Context.CanDeleteEntityFunc(entity));

    // Client side paging/filtering
    private bool LocalSearch(TEntity entity)
    {
        if (Context is not EntityClientTableContext<TEntity, TId, TRequest> clientContext
            || clientContext.SearchFunc is null)
        {
            return string.IsNullOrWhiteSpace(SearchString);
        }

        return clientContext.SearchFunc(SearchString, entity);
    }

    private async Task LoadDataAsync()
    {
        if (Loading || Context is not EntityClientTableContext<TEntity, TId, TRequest> clientContext)
        {
            return;
        }

        Loading = true;

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => clientContext.LoadDataFunc(), Snackbar)
            is List<TEntity> result)
        {
            _entityList = result;
        }

        Loading = false;
    }

    // Server Side paging/filtering
    private async Task OnSearch(string? text = null)
    {
        await SearchStringChanged.InvokeAsync(SearchString);

        if (Context is EntityServerTableContext<TEntity, TId, TRequest>)
        {
            _table?.ReloadServerData();
        }
    }

    private Func<TableState, Task<TableData<TEntity>>>? ServerReloadFunc =>
        Context is EntityServerTableContext<TEntity, TId, TRequest>
            ? ServerReload : null;

    private async Task<TableData<TEntity>> ServerReload(TableState state)
    {
        if (!string.IsNullOrWhiteSpace(SearchString))
        {
            state.Page = 0;
        }

        await LoadDataAsync(state);

        return new TableData<TEntity> { TotalItems = _totalItems, Items = _entityList };
    }

    private async Task LoadDataAsync(TableState state)
    {
        if (Loading || Context is not EntityServerTableContext<TEntity, TId, TRequest> serverContext)
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
            Keyword = SearchString,
            OrderBy = orderings ?? Array.Empty<string>()
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => serverContext.SearchFunc(filter), Snackbar)
            is PaginatedResult<TEntity> result)
        {
            _totalItems = result.TotalCount;
            _entityList = result.Data;
        }

        Loading = false;
    }

    private async Task InvokeModal(TEntity? entity = default)
    {
        bool isCreate = entity is null;

        string title = isCreate
            ? L["Create "] + Context.EntityName
            : L["Update "] + Context.EntityName;

        var parameters = new DialogParameters()
        {
            { nameof(AddEditModal<TRequest>.EditFormContent), EditFormContent },
            { nameof(AddEditModal<TRequest>.OnInitializedFunc), Context.EditFormInitializedFunc },
            { nameof(AddEditModal<TRequest>.IsCreate), isCreate },
            { nameof(AddEditModal<TRequest>.Title), title }
        };

        if (isCreate)
        {
            _ = Context.CreateFunc ?? throw new InvalidOperationException("CreateFunc can't be null!");
            parameters.Add(nameof(AddEditModal<TRequest>.SaveFunc), Context.CreateFunc);

            var requestModel =
                Context.GetDefaultsFunc is not null
                    && await ApiHelper.ExecuteCallGuardedAsync(
                            () => Context.GetDefaultsFunc(), Snackbar)
                        is TRequest defaultsResult
                ? defaultsResult
                : new TRequest();
            parameters.Add(nameof(AddEditModal<TRequest>.RequestModel), requestModel);
        }
        else
        {
            _ = Context.IdFunc ?? throw new InvalidOperationException("IdFunc can't be null!");
            var id = Context.IdFunc(entity!);
            parameters.Add(nameof(AddEditModal<TRequest>.Id), id);

            _ = Context.UpdateFunc ?? throw new InvalidOperationException("UpdateFunc can't be null!");
            Func<TRequest, Task> saveFunc = entity => Context.UpdateFunc(id, entity);
            parameters.Add(nameof(AddEditModal<TRequest>.SaveFunc), saveFunc);

            var requestModel =
                Context.GetDetailsFunc is not null
                    && await ApiHelper.ExecuteCallGuardedAsync(
                            () => Context.GetDetailsFunc(id!),
                            Snackbar)
                        is TRequest detailsResult
                ? detailsResult
                : entity!.Adapt<TRequest>();
            parameters.Add(nameof(AddEditModal<TRequest>.RequestModel), requestModel);
        }

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };

        var dialog = DialogService.Show<AddEditModal<TRequest>>(isCreate ? L["Create"] : L["Edit"], parameters, options);

        Context.SetAddEditModalRef(dialog);

        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            await ResetAsync();
        }
    }

    private async Task Delete(TEntity entity)
    {
        _ = Context.IdFunc ?? throw new InvalidOperationException("IdFunc can't be null!");
        TId id = Context.IdFunc(entity);

        string deleteContent = L["You're sure you want to delete {0} with id '{1}'?"];
        var parameters = new DialogParameters
        {
            { nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, Context.EntityName, id) }
        };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<DeleteConfirmation>(L["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            _ = Context.DeleteFunc ?? throw new InvalidOperationException("DeleteFunc can't be null!");

            await ApiHelper.ExecuteCallGuardedAsync(
                () => Context.DeleteFunc(id),
                Snackbar);

            await ResetAsync();
        }
    }

    private Task ResetAsync()
    {
        if (Context is EntityClientTableContext<TEntity, TId, TRequest>)
        {
            return LoadDataAsync();
        }
        else
        {
            SearchString = string.Empty;
            return OnSearch();
        }
    }
}