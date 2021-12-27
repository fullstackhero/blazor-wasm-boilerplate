using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Components.EntityManager;

public class ClientEntityManagerContext<TEntity, TId> : EntityManagerContext<TEntity, TId>
{
    public Func<Task<ListResult<TEntity>>> LoadDataFunc { get; }
    public Func<string?, TEntity, bool> SearchFunc { get; }

    public ClientEntityManagerContext(
        List<EntityField<TEntity>> fields,
        Func<Task<ListResult<TEntity>>> loadDataFunc,
        Func<string?, TEntity, bool> searchFunc,
        string searchPermission,
        Func<TEntity, TId>? idFunc = null,
        Func<TEntity, Task<Result>>? createFunc = null,
        Func<TEntity, Task<Result>>? updateFunc = null,
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
            createFunc,
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