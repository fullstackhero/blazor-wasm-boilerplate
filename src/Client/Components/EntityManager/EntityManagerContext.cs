using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Components.EntityManager;

public record EntityField<T>(string SortLabel, string DataLabel, Func<T, object?> DataValue, RenderFragment<T>? Template = null);

public abstract class EntityManagerContext<TEntity>
{
    public List<EntityField<TEntity>> Fields { get; }
    public string SearchPermission { get; }
    public Func<TEntity, object?>? IdFunc { get; }
    public Func<TEntity, Task<Result>>? CreateFunc { get; }
    public Func<TEntity, Task<Result>>? UpdateFunc { get; }
    public Func<object?, Task<Result>>? DeleteFunc { get; }
    public string? CreatePermission { get; }
    public string? UpdatePermission { get; }
    public string? DeletePermission { get; }
    public string? EntityName { get; }
    public string? EntityNamePlural { get; }
    public Func<Task>? EditFormInitializedFunc { get; }
    public Func<bool>? HasExtraActionsFunc { get; set; }

    public EntityManagerContext(
        List<EntityField<TEntity>> fields,
        string searchPermission,
        Func<TEntity, object?>? idFunc,
        Func<TEntity, Task<Result>>? createFunc,
        Func<TEntity, Task<Result>>? updateFunc,
        Func<object?, Task<Result>>? deleteFunc,
        string? createPermission,
        string? updatePermission,
        string? deletePermission,
        string? entityName,
        string? entityNamePlural,
        Func<Task>? editFormInitializedFunc = null,
        Func<bool>? hasExtraActionsFunc = null)
    {
        Fields = fields;
        SearchPermission = searchPermission;
        IdFunc = idFunc;
        CreateFunc = createFunc;
        UpdateFunc = updateFunc;
        DeleteFunc = deleteFunc;
        CreatePermission = createPermission;
        UpdatePermission = updatePermission;
        DeletePermission = deletePermission;
        EntityName = entityName;
        EntityNamePlural = entityNamePlural;
        EditFormInitializedFunc = editFormInitializedFunc;
        HasExtraActionsFunc = hasExtraActionsFunc;
    }
}