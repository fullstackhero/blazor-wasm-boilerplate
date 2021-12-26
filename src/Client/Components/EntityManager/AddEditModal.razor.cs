using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.EntityManager;

public partial class AddEditModal<TEntity>
    where TEntity : new()
{
    [Parameter]
    [EditorRequired]
    public EntityManagerContext<TEntity> Context { get; set; } = default!;
    [Parameter]
    [EditorRequired]
    public RenderFragment<TEntity> EditFormContent { get; set; } = default!;

    [Parameter]
    public TEntity EntityModel { get; set; } = new();
    [Parameter]
    public bool IsCreate { get; set; }
    [Parameter]
    public object? Id { get; set; }

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    private CustomValidation? _customValidation;

    protected override Task OnInitializedAsync() =>
        Context.EditFormInitializedFunc is not null
            ? Context.EditFormInitializedFunc()
            : Task.CompletedTask;

    private async Task SaveAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => IsCreate
                ? Context.CreateFunc(EntityModel)
                : Context.UpdateFunc(EntityModel),
            _snackBar,
            _customValidation,
            L["Success"]) is not null)
        {
            MudDialog.Close();
        }
    }

    private void Cancel() =>
        MudDialog.Cancel();
}