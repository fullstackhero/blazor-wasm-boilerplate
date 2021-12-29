using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.ComponentModel.DataAnnotations;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

public partial class AddEditModal<TRequest> : IAddEditModal
{
    [Parameter]
    [EditorRequired]
    public RenderFragment<TRequest> EditFormContent { get; set; } = default!;
    [Parameter]
    [EditorRequired]
    public TRequest RequestModel { get; set; } = default!;
    [Parameter]
    [EditorRequired]
    public Func<TRequest, Task<Result>> SaveFunc { get; set; } = default!;

    [Parameter]
    public Func<Task>? OnInitializedFunc { get; set; }

    [Parameter]
    [EditorRequired]
    public string Title { get; set; } = default!;
    [Parameter]
    public bool IsCreate { get; set; }
    [Parameter]
    public object? Id { get; set; }

    [CascadingParameter]
    private MudDialogInstance _mudDialog { get; set; } = default!;

    private CustomValidation? _customValidation;

    protected override Task OnInitializedAsync() =>
        OnInitializedFunc is not null
            ? OnInitializedFunc()
            : Task.CompletedTask;

    public void ForceRender() =>
        StateHasChanged();

    // This should not be necessary anymore, except maybe in the case when the
    // UpdateEntityRequest has different validation rules than the CreateEntityRequest.
    // If that would happen a lot we can still change the design so this method doesn't need to be called manually.
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
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => SaveFunc(RequestModel),
                _snackBar,
                _customValidation,
                L["Success"])
            is not null)
        {
            _mudDialog.Close();
        }
    }

    private void Cancel() =>
        _mudDialog.Cancel();
}