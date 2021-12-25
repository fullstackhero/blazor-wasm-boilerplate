using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Components.EntityManager;

public record EntityField<T>(string SortLabel, string DataLabel, Func<T, object?> DataValue);

public record PaginatedResult<T>(bool Succeeded, List<T>? Data, List<string>? Messages, int TotalCount = 0, int CurrentPage = 1, int PageSize = 10);

public class EntityManagerContext<TEntity, TFilter>
{
    public List<EntityField<TEntity>> Fields { get; }
    public Func<TEntity, Guid> IdFunc { get; }
    public Func<TFilter, Task<PaginatedResult<TEntity>>> SearchFunc { get; }
    public Func<TEntity, Task<ResultOfGuid>> CreateFunc { get; }
    public Func<TEntity, Task<ResultOfGuid>> UpdateFunc { get; }
    public Func<Guid, Task<ResultOfGuid>> DeleteFunc { get; }
    public Func<Task>? EditFormInitializedFunc { get; }
    public string EntityName { get; }
    public string EntityNamePlural { get; }
    public string SearchPermission { get; }
    public string CreatePermission { get; }
    public string UpdatePermission { get; }
    public string DeletePermission { get; }

    public EntityManagerContext(
        List<EntityField<TEntity>> fields,
        Func<TEntity, Guid> idFunc,
        Func<TFilter, Task<PaginatedResult<TEntity>>> searchFunc,
        Func<TEntity, Task<ResultOfGuid>> createFunc,
        Func<TEntity, Task<ResultOfGuid>> updateFunc,
        Func<Guid, Task<ResultOfGuid>> deleteFunc,
        Func<Task>? editFormInitializedFunc,
        string entityName,
        string entityNamePlural,
        string searchPermission,
        string createPermission,
        string updatePermission,
        string deletePermission)
    {
        Fields = fields;
        IdFunc = idFunc;
        SearchFunc = searchFunc;
        CreateFunc = createFunc;
        UpdateFunc = updateFunc;
        DeleteFunc = deleteFunc;
        EditFormInitializedFunc = editFormInitializedFunc;
        EntityName = entityName;
        EntityNamePlural = entityNamePlural;
        SearchPermission = searchPermission;
        CreatePermission = createPermission;
        UpdatePermission = updatePermission;
        DeletePermission = deletePermission;
    }
}