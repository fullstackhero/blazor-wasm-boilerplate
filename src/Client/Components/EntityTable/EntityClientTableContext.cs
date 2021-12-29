using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

/// <summary>
/// Initialization Context for the EntityTable Component.
/// Use this one if you want to use Client Paging, Sorting and Filtering.
/// </summary>
public class EntityClientTableContext<TEntity, TId, TRequest>
    : EntityTableContext<TEntity, TId, TRequest>
{
    /// <summary>
    /// A function that loads all the data for the table from the api and returns a ListResult of TEntity.
    /// </summary>
    public Func<Task<ListResult<TEntity>>> LoadDataFunc { get; }

    /// <summary>
    /// A function that returns a boolean which indicates whether the supplied entity meets the search criteria
    /// (the supplied string is the search string entered).
    /// </summary>
    public Func<string?, TEntity, bool> SearchFunc { get; }

    public EntityClientTableContext(
        List<EntityField<TEntity>> fields,
        Func<Task<ListResult<TEntity>>> loadDataFunc,
        Func<string?, TEntity, bool> searchFunc,
        string searchPermission,
        Func<TEntity, TId>? idFunc = null,
        Func<Task<Result<TRequest>>>? getDefaultsFunc = null,
        Func<TRequest, Task<Result>>? createFunc = null,
        Func<TId, Task<Result<TRequest>>>? getDetailsFunc = null,
        Func<TId, TRequest, Task<Result>>? updateFunc = null,
        Func<TId, Task<Result>>? deleteFunc = null,
        string? createPermission = null,
        string? updatePermission = null,
        string? deletePermission = null,
        string? entityName = null,
        string? entityNamePlural = null,
        Func<Task>? editFormInitializedFunc = null,
        Func<bool>? hasExtraActionsFunc = null)
        : base(
            fields,
            searchPermission,
            idFunc,
            getDefaultsFunc,
            createFunc,
            getDetailsFunc,
            updateFunc,
            deleteFunc,
            createPermission,
            updatePermission,
            deletePermission,
            entityName,
            entityNamePlural,
            editFormInitializedFunc,
            hasExtraActionsFunc)
    {
        LoadDataFunc = loadDataFunc;
        SearchFunc = searchFunc;
    }
}