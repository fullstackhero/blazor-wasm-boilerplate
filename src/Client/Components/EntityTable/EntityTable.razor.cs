using AKSoftware.Blazor.Utilities;
using FSH.BlazorWebAssembly.Client.Components.ThemeManager;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Preferences;
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
    public bool Striped { get; set; }
    [Parameter]
    public bool Bordered { get; set; }
    [Parameter]
    public bool Hoverable { get; set; }
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

    private MudTable<TEntity> _table = default!;
    private IEnumerable<TEntity>? _entityList;
    private int _totalItems;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canSearch = await CanDoPermission(Context.SearchPermission, state);
        _canCreate = await CanDoPermission(Context.CreatePermission, state);
        _canUpdate = await CanDoPermission(Context.UpdatePermission, state);
        _canDelete = await CanDoPermission(Context.DeletePermission, state);

        await LocalLoadDataAsync();
        await SetAndSubscribeToTablePreference();
    }

    private async Task SetAndSubscribeToTablePreference()
    {
        if (await ClientPreferences.GetPreference() is ClientPreference clientPreference)
        {
            SetTablePreference(clientPreference.EntityTablePreference);
            MessagingCenter.Subscribe<TableCustomizationPanel, EntityTablePreference>(this, nameof(ClientPreference.EntityTablePreference), (_, value) =>
                {
                    SetTablePreference(value);
                    StateHasChanged();
                });
        }
    }

    private void SetTablePreference(EntityTablePreference tablePreference)
    {
        Dense = tablePreference.IsDense;
        Striped = tablePreference.IsStriped;
        Bordered = tablePreference.HasBorder;
        Hoverable = tablePreference.IsHoverable;
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
        if (Context.ClientContext?.SearchFunc is null)
        {
            return string.IsNullOrWhiteSpace(SearchString);
        }

        return Context.ClientContext.SearchFunc(SearchString, entity);
    }

    private async Task LocalLoadDataAsync()
    {
        if (Loading || Context.ClientContext is null)
        {
            return;
        }

        Loading = true;

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => Context.ClientContext.LoadDataFunc(), Snackbar)
            is List<TEntity> result)
        {
            _entityList = result;
        }

        Loading = false;
    }

    private async Task OnSearchStringChanged(string? text = null)
    {
        await SearchStringChanged.InvokeAsync(SearchString);

        await ServerLoadDataAsync();
    }

    // Server Side paging/filtering
    private async Task ServerLoadDataAsync()
    {
        if (Context.IsServerContext)
        {
            await _table.ReloadServerData();
        }
    }

    private Func<TableState, Task<TableData<TEntity>>>? ServerReloadFunc =>
        Context.IsServerContext ? ServerReload : null;

    private async Task<TableData<TEntity>> ServerReload(TableState state)
    {
        if (!Loading && Context.ServerContext is not null)
        {
            Loading = true;

            var filter = GetPaginationFilter(state);

            if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => Context.ServerContext.SearchFunc(filter), Snackbar)
                is PaginationResponse<TEntity> result)
            {
                _totalItems = result.TotalCount;
                _entityList = result.Data;
            }

            Loading = false;
        }

        return new TableData<TEntity> { TotalItems = _totalItems, Items = _entityList };
    }

    private PaginationFilter GetPaginationFilter(TableState state)
    {
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

        if (!Context.AllColumnsChecked)
        {
            filter.AdvancedSearch = new()
            {
                Fields = Context.SearchFields,
                Keyword = filter.Keyword
            };
            filter.Keyword = null;
        }

        return filter;
    }

    private async Task InvokeModal(TEntity? entity = default)
    {
        bool isCreate = entity is null;

        var parameters = new DialogParameters()
        {
            { nameof(AddEditModal<TRequest>.EditFormContent), EditFormContent },
            { nameof(AddEditModal<TRequest>.OnInitializedFunc), Context.EditFormInitializedFunc },
            { nameof(AddEditModal<TRequest>.EntityName), Context.EntityName }
        };

        TRequest requestModel;

        if (isCreate)
        {
            _ = Context.CreateFunc ?? throw new InvalidOperationException("CreateFunc can't be null!");
            parameters.Add(nameof(AddEditModal<TRequest>.SaveFunc), Context.CreateFunc);

            requestModel =
                Context.GetDefaultsFunc is not null
                    && await ApiHelper.ExecuteCallGuardedAsync(
                            () => Context.GetDefaultsFunc(), Snackbar)
                        is TRequest defaultsResult
                ? defaultsResult
                : new TRequest();
        }
        else
        {
            _ = Context.IdFunc ?? throw new InvalidOperationException("IdFunc can't be null!");
            var id = Context.IdFunc(entity!);
            parameters.Add(nameof(AddEditModal<TRequest>.Id), id);

            _ = Context.UpdateFunc ?? throw new InvalidOperationException("UpdateFunc can't be null!");
            Func<TRequest, Task> saveFunc = entity => Context.UpdateFunc(id, entity);
            parameters.Add(nameof(AddEditModal<TRequest>.SaveFunc), saveFunc);

            requestModel =
                Context.GetDetailsFunc is not null
                    && await ApiHelper.ExecuteCallGuardedAsync(
                            () => Context.GetDetailsFunc(id!),
                            Snackbar)
                        is TRequest detailsResult
                ? detailsResult
                : entity!.Adapt<TRequest>();
        }

        parameters.Add(nameof(AddEditModal<TRequest>.RequestModel), requestModel);

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };

        var dialog = DialogService.Show<AddEditModal<TRequest>>(isCreate ? L["Create"] : L["Edit"], parameters, options);

        Context.SetAddEditModalRef(dialog);

        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            await ReloadDataAsync();
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

            await ReloadDataAsync();
        }
    }

    public Task ReloadDataAsync()
    {
        if (Context.IsClientContext)
        {
            return LocalLoadDataAsync();
        }
        else
        {
            return ServerLoadDataAsync();
        }
    }
}