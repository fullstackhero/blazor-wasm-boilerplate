using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

/// <summary>
/// Abstract base class for the initialization Context of the EntityTable Component.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the id of the entity.</typeparam>
/// <typeparam name="TRequest">The type of the Request which is used on the AddEditModal and which is sent with the CreateFunc and UpdateFunc.</typeparam>
public abstract class EntityTableContext<TEntity, TId, TRequest>
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
    /// A function that executes the GetDefaults method on the api (or supplies defaults locally) and returns
    /// a Task of Result of TRequest. When not supplied, a TRequest is simply newed up.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<Task<TRequest>>? GetDefaultsFunc { get; }

    /// <summary>
    /// A function that executes the Create method on the api with the supplied entity and returns a Task of Result.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<TRequest, Task>? CreateFunc { get; }

    /// <summary>
    /// A function that executes the GetDetails method on the api with the supplied Id and returns a Task of Result of TRequest.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// When not supplied, the TEntity out of the _entityList is supplied using the IdFunc and converted using mapster.
    /// </summary>
    public Func<TId, Task<TRequest>>? GetDetailsFunc { get; }

    /// <summary>
    /// A function that executes the Update method on the api with the supplied entity and returns a Task of Result.
    /// When not supplied, the TEntity from the list is mapped to TCreateRequest using mapster.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<TId, TRequest, Task>? UpdateFunc { get; }

    /// <summary>
    /// A function that executes the Delete method on the api with the supplied entity id and returns a Task of Result.
    /// No need to check for error messages or api exceptions. These are automatically handled by the component.
    /// </summary>
    public Func<TId, Task>? DeleteFunc { get; }

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

    /// <summary>
    /// Use this if you want to disable the update functionality for specific entities in the table.
    /// </summary>
    public Func<TEntity, bool>? CanUpdateEntityFunc { get; set; }

    /// <summary>
    /// Use this if you want to disable the delete functionality for specific entities in the table.
    /// </summary>
    public Func<TEntity, bool>? CanDeleteEntityFunc { get; set; }

    public EntityTableContext(
        List<EntityField<TEntity>> fields,
        string searchPermission,
        Func<TEntity, TId>? idFunc,
        Func<Task<TRequest>>? getDefaultsFunc,
        Func<TRequest, Task>? createFunc,
        Func<TId, Task<TRequest>>? getDetailsFunc,
        Func<TId, TRequest, Task>? updateFunc,
        Func<TId, Task>? deleteFunc,
        string? createPermission,
        string? updatePermission,
        string? deletePermission,
        string? entityName,
        string? entityNamePlural,
        Func<Task>? editFormInitializedFunc,
        Func<bool>? hasExtraActionsFunc,
        Func<TEntity, bool>? canUpdateEntityFunc,
        Func<TEntity, bool>? canDeleteEntityFunc)
    {
        Fields = fields;
        SearchPermission = searchPermission;
        IdFunc = idFunc;
        GetDefaultsFunc = getDefaultsFunc;
        CreateFunc = createFunc;
        GetDetailsFunc = getDetailsFunc;
        UpdateFunc = updateFunc;
        DeleteFunc = deleteFunc;
        CreatePermission = createPermission;
        UpdatePermission = updatePermission;
        DeletePermission = deletePermission;
        EntityName = entityName;
        EntityNamePlural = entityNamePlural;
        EditFormInitializedFunc = editFormInitializedFunc;
        HasExtraActionsFunc = hasExtraActionsFunc;
        CanUpdateEntityFunc = canUpdateEntityFunc;
        CanDeleteEntityFunc = canDeleteEntityFunc;
    }

    private IDialogReference? _addEditModalRef;

    internal void SetAddEditModalRef(IDialogReference dialog) =>
        _addEditModalRef = dialog;

    public IAddEditModal? AddEditModal =>
        _addEditModalRef?.Dialog as IAddEditModal;
}