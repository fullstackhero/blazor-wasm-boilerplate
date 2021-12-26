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
        Func<TEntity, object?> idFunc,
        Func<PaginationFilter, Task<PaginatedResult<TEntity>>> searchFunc,
        Func<TEntity, Task<Result>> createFunc,
        Func<TEntity, Task<Result>> updateFunc,
        Func<object?, Task<Result>> deleteFunc,
        string entityName,
        string entityNamePlural,
        string searchPermission,
        string createPermission,
        string updatePermission,
        string deletePermission,
        Func<Task>? editFormInitializedFunc = null,
        Func<bool>? hasExtraActionsFunc = null)
        : base(
            fields,
            idFunc,
            createFunc,
            updateFunc,
            deleteFunc,
            entityName,
            entityNamePlural,
            searchPermission,
            createPermission,
            updatePermission,
            deletePermission,
            editFormInitializedFunc,
            hasExtraActionsFunc) =>
        SearchFunc = searchFunc;
}