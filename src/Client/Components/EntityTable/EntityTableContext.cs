using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

/// <summary>
/// Abstract base class for the initialization Context of the EntityTable Component.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the id of the entity.</typeparam>
public abstract class EntityTableContext<TEntity, TId>
    where TEntity : new()
{
    /// <summary>
    /// The columns you want to display on the table.
    /// </summary>
    public List<EntityField<TEntity>> Fields { get; }

    /// <summary>
    /// The permission name of the search permission. When empty, no search functionality will be available.
    /// When the string "true", search funtionality will be enabled, otherwise it will only be enabled if the
    /// user has the permission specified.
    /// </summary>
    public string SearchPermission { get; }

    /// <summary>
    /// A function that returns the Id of the entity. This is only needed when using the CRUD functionality.
    /// </summary>
    public Func<TEntity, TId>? IdFunc { get; }

    /// <summary>
    /// A function that executes the Create method on the api with the supplied entity and returns a Task of Result.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<TEntity, Task<Result>>? CreateFunc { get; }

    /// <summary>
    /// A function that executes the Update method on the api with the supplied entity and returns a Task of Result.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<TEntity, Task<Result>>? UpdateFunc { get; }

    /// <summary>
    /// A function that executes the Delete method on the api with the supplied entity id and returns a Task of Result.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<TId, Task<Result>>? DeleteFunc { get; }

    /// <summary>
    /// The permission name of the create permission. When empty, no create functionality will be available.
    /// When the string "true", create funtionality will be enabled, otherwise it will only be enabled if the
    /// user has the permission specified.
    /// </summary>
    public string? CreatePermission { get; }

    /// <summary>
    /// The permission name of the update permission. When empty, no update functionality will be available.
    /// When the string "true", update funtionality will be enabled, otherwise it will only be enabled if the
    /// user has the permission specified.
    /// </summary>
    public string? UpdatePermission { get; }

    /// <summary>
    /// The permission name of the delete permission. When empty, no delete functionality will be available.
    /// When the string "true", delete funtionality will be enabled, otherwise it will only be enabled if the
    /// user has the permission specified.
    /// </summary>
    public string? DeletePermission { get; }

    /// <summary>
    /// The name of the entity. This is used in the title of the add/edit modal and delete confirmation.
    /// </summary>
    public string? EntityName { get; }

    /// <summary>
    /// The plural name of the entity. This is used in the "Search for ..." placeholder.
    /// </summary>
    public string? EntityNamePlural { get; }

    /// <summary>
    /// Use this if you want to run initialization during OnInitialized of the AddEdit form.
    /// </summary>
    public Func<Task>? EditFormInitializedFunc { get; }

    /// <summary>
    /// Use this if you want to check for permissions of content in the ExtraActions RenderFragment.
    /// The extra actions won't be available when this returns false.
    /// </summary>
    public Func<bool>? HasExtraActionsFunc { get; set; }

    public EntityTableContext(
        List<EntityField<TEntity>> fields,
        string searchPermission,
        Func<TEntity, TId>? idFunc,
        Func<TEntity, Task<Result>>? createFunc,
        Func<TEntity, Task<Result>>? updateFunc,
        Func<TId, Task<Result>>? deleteFunc,
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

    private IDialogReference? _addEditModalRef;

    internal void SetAddEditModalRef(IDialogReference dialog) =>
        _addEditModalRef = dialog;

    public AddEditModal<TEntity, TId>? AddEditModal =>
        _addEditModalRef?.Dialog as AddEditModal<TEntity, TId>;
}