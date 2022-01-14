using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity.Account;

public partial class Security
{
    [Inject]
    public IIdentityClient IdentityClient { get; set; } = default!;

    private readonly ChangePasswordRequest _passwordModel = new();

    private CustomValidation? _customValidation;

    private async Task ChangePasswordAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => IdentityClient.ChangePasswordAsync(_passwordModel),
            Snackbar,
            _customValidation,
            L["Password Changed!"]))
        {
            _passwordModel.Password = string.Empty;
            _passwordModel.NewPassword = string.Empty;
            _passwordModel.ConfirmNewPassword = string.Empty;
        }
    }

    private InputType _currentPasswordInput = InputType.Password;
    private string _currentPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
    private bool _newPasswordVisibility;
    private InputType _newPasswordInput = InputType.Password;
    private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private void TogglePasswordVisibility(bool newPassword)
    {
        if (newPassword)
        {
            if (_newPasswordVisibility)
            {
                _newPasswordVisibility = false;
                _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                _newPasswordInput = InputType.Password;
            }
            else
            {
                _newPasswordVisibility = true;
                _newPasswordInputIcon = Icons.Material.Filled.Visibility;
                _newPasswordInput = InputType.Text;
            }
        }
    }
}