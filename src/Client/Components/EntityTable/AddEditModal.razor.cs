using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

public partial class AddEditModal<TEntity, TId>
    where TEntity : new()
{
    [Parameter]
    [EditorRequired]
    public EntityTableContext<TEntity, TId> Context { get; set; } = default!;
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

    public void ForceRender() =>
        StateHasChanged();

    // This is temporary... we should probably use another type parameter here for the request type.
    public Result Validate(object request)
    {
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(request, new ValidationContext(request), results, true))
        {
            // Convert results to errors
            var errors = new Dictionary<string, ICollection<string>>();
            foreach (var result in results
                .Where(r => !string.IsNullOrWhiteSpace(r.ErrorMessage)))
            {
                foreach (string field in result.MemberNames)
                {
                    if (errors.ContainsKey(field))
                    {
                        errors[field].Add(result.ErrorMessage!);
                    }
                    else
                    {
                        errors.Add(field, new List<string>() { result.ErrorMessage! });
                    }
                }
            }

            _customValidation?.DisplayErrors(errors);

            return new Result { Succeeded = false, Messages = new List<string>() { "Validation failed." } };
        }

        return new Result { Succeeded = true };
    }

    private async Task SaveAsync()
    {
        if (IsCreate)
        {
            _ = Context.CreateFunc ?? throw new InvalidOperationException("CreateFunc can't be null!");
        }
        else
        {
            _ = Context.UpdateFunc ?? throw new InvalidOperationException("UpdateFunc can't be null!");
        }

        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => IsCreate
                ? Context.CreateFunc!(EntityModel)
                : Context.UpdateFunc!(EntityModel),
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