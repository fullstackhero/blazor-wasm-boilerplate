using FSH.BlazorWebAssembly.Client.Components.Common;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.WebApi.Shared.Multitenancy;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Authentication;

public partial class Register
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

        //if (!PasswordConfirm.Equals(_createUserRequest.Password, StringComparison.CurrentCulture))
        //{
        //    // FAIL
        //    return;
        //}

        BusySubmitting = true;
        
        await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.CreateAsync(Tenant, _createUserRequest),
            Snackbar, _customValidation);

        Navigation.NavigateTo("/login");

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