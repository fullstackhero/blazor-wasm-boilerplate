using FL_CRMS_ERP_WASM.Client.Components.Common;
using FL_CRMS_ERP_WASM.Client.Infrastructure.ApiClient;
using FL_CRMS_ERP_WASM.Client.Shared;
using FL.WebApi.Shared.Multitenancy;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FL_CRMS_ERP_WASM.Client.Pages.Authentication;

public partial class SelfRegister
{
    private readonly CreateUserRequest _createUserRequest = new();
    private CustomValidation? _customValidation;
    private bool BusySubmitting { get; set; }

    [Inject]
    private IUsersClient UsersClient { get; set; } = default!;

    private string Tenant { get; set; } = MultitenancyConstants.Root.Id;

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        string? sucessMessage = await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.SelfRegisterAsync(Tenant, _createUserRequest),
            Snackbar,
            _customValidation);

        if (sucessMessage != null)
        {
            Snackbar.Add(sucessMessage, Severity.Info);
            Navigation.NavigateTo("/login");
        }

        BusySubmitting = false;
    }

    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }
    }
}