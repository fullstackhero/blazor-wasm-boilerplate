using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Shared;

public static class ApiHelper
{
    public static async Task<T?> ExecuteCallGuardedAsync<T>(
        Func<Task<T>> call,
        ISnackbar snackbar,
        CustomValidation? customValidation = null,
        string? successMessage = null)
    where T : Result
    {
        customValidation?.ClearErrors();
        try
        {
            var result = await call();

            if (result.Succeeded)
            {
                if (result.Messages is not null)
                {
                    foreach (string message in result.Messages)
                    {
                        snackbar.Add(message, Severity.Success);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(successMessage))
                {
                    snackbar.Add(successMessage, Severity.Success);
                }

                return result;
            }

            if (result.Messages is not null)
            {
                foreach (string message in result.Messages)
                {
                    snackbar.Add(message, Severity.Error);
                }
            }
            else
            {
                snackbar.Add("Something went wrong!", Severity.Error);
            }
        }
        catch (ApiException<HttpValidationProblemDetails> ex)
        {
            customValidation?.DisplayErrors(ex.Result.Errors);
        }
        catch (ApiException<ErrorResult> ex)
        {
            snackbar.Add(ex.Result.Exception, Severity.Error);
            if (ex.Result.ValidationErrors is not null)
            {
                customValidation?.DisplayErrors(ex.Result.ValidationErrors);
            }
        }

        return default;
    }
}