using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Components.EntityManager;

public class ListResult<T> : Result
{
    public List<T>? Data { get; set; }
}

public class PaginatedResult<T> : ListResult<T>
{
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PaginationFilter
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public string? Keyword { get; set; }
    public string[]? OrderBy { get; set; }
}

public class ServerEntityManagerContext<TEntity> : EntityManagerContext<TEntity>
{
    public Func<PaginationFilter, Task<PaginatedResult<TEntity>>> SearchFunc { get; }

    public ServerEntityManagerContext(
        List<EntityField<TEntity>> fields,
        Func<PaginationFilter, Task<PaginatedResult<TEntity>>> searchFunc,
        string searchPermission,
        Func<TEntity, object?>? idFunc,
        Func<TEntity, Task<Result>>? createFunc,
        Func<TEntity, Task<Result>>? updateFunc,
        Func<object?, Task<Result>>? deleteFunc,
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