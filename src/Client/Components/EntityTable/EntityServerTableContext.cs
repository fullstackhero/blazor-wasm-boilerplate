using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

/// <summary>
/// Initialization Context for the EntityTable Component.
/// Use this one if you want to use Server Paging, Sorting and Filtering.
/// </summary>
public class EntityServerTableContext<TEntity, TId, TRequest>
    : EntityTableContext<TEntity, TId, TRequest>
{
    /// <summary>
    /// A function that loads the specified page from the api with the specified search criteria
    /// and returns a PaginatedResult of TEntity.
    /// </summary>
    public Func<PaginationFilter, Task<PaginatedResult<TEntity>>> SearchFunc { get; }

    public EntityServerTableContext(
        List<EntityField<TEntity>> fields,
        Func<PaginationFilter, Task<PaginatedResult<TEntity>>> searchFunc,
        string searchPermission,
        Func<TEntity, TId>? idFunc = null,
        Func<Task<Result<TRequest>>>? getDefaultsFunc = null,
        Func<TRequest, Task<Result>>? createFunc = null,
        Func<TId, Task<Result<TRequest>>>? getDetailsFunc = null,
        Func<TId, TRequest, Task<Result>>? updateFunc = null,
        Func<TId, Task<Result>>? deleteFunc = null,
        string? entityName = null,
        string? entityNamePlural = null,
        string? createPermission = null,
        string? updatePermission = null,
        string? deletePermission = null,
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
            hasExtraActionsFunc) =>
        SearchFunc = searchFunc;
}