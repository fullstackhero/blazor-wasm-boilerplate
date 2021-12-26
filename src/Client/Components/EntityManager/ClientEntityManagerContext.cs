using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Components.EntityManager;

public class ClientEntityManagerContext<TEntity> : EntityManagerContext<TEntity>
{
    public Func<Task<ListResult<TEntity>>> LoadDataFunc { get; }
    public Func<string, TEntity, bool> LocalSearchFunc { get; }

    public ClientEntityManagerContext(
        List<EntityField<TEntity>> fields,
        Func<TEntity, object?> idFunc,
        Func<Task<ListResult<TEntity>>> loadDataFunc,
        Func<string, TEntity, bool> localSearchFunc,
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
            hasExtraActionsFunc)
    {
        LoadDataFunc = loadDataFunc;
        LocalSearchFunc = localSearchFunc;
    }
}