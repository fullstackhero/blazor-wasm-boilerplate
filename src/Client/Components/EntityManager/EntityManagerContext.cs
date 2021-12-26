using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Components.EntityManager;

public record EntityField<T>(string SortLabel, string DataLabel, Func<T, object?> DataValue);

public abstract class EntityManagerContext<TEntity>
{
    public List<EntityField<TEntity>> Fields { get; }
    public Func<TEntity, object?> IdFunc { get; }
    public Func<TEntity, Task<Result>> CreateFunc { get; }
    public Func<TEntity, Task<Result>> UpdateFunc { get; }
    public Func<object?, Task<Result>> DeleteFunc { get; }
    public string EntityName { get; }
    public string EntityNamePlural { get; }
    public string SearchPermission { get; }
    public string CreatePermission { get; }
    public string UpdatePermission { get; }
    public string DeletePermission { get; }
    public Func<Task>? EditFormInitializedFunc { get; }
    public Func<bool>? HasExtraActionsFunc { get; set; }

    public EntityManagerContext(
        List<EntityField<TEntity>> fields,
        Func<TEntity, object?> idFunc,
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
    {
        Fields = fields;
        IdFunc = idFunc;
        CreateFunc = createFunc;
        UpdateFunc = updateFunc;
        DeleteFunc = deleteFunc;
        EntityName = entityName;
        EntityNamePlural = entityNamePlural;
        SearchPermission = searchPermission;
        CreatePermission = createPermission;
        UpdatePermission = updatePermission;
        DeletePermission = deletePermission;
        EditFormInitializedFunc = editFormInitializedFunc;
        HasExtraActionsFunc = hasExtraActionsFunc;
    }
}