using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

/// <summary>
/// Initialization Context for the EntityTable Component.
/// Use this one if you want to use Server Paging, Sorting and Filtering.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the id of the entity.</typeparam>
public class EntityServerTableContext<TEntity, TId> : EntityTableContext<TEntity, TId>
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
        Func<TEntity, TId>? idFunc,
        Func<TEntity, Task<Result>>? createFunc,
        Func<TEntity, Task<Result>>? updateFunc,
        Func<TId, Task<Result>>? deleteFunc,
        string? entityName,
        string? entityNamePlural,
        string? createPermission,
        string? updatePermission,
        string? deletePermission,
        Func<Task>? editFormInitializedFunc = null,
        Func<bool>? hasExtraActionsFunc = null)
        : base(
            fields,
            searchPermission,
            idFunc,
            createFunc,
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