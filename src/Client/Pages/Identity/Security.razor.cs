using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity;

public partial class Security
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    public IIdentityClient IdentityClient { get; set; } = default!;

    private readonly ChangePasswordRequest _passwordModel = new();
    private string? ConfirmationPassword { get; set; }

    private async Task ChangePasswordAsync()
    {
        var response = await IdentityClient.ChangePasswordAsync(_passwordModel);
        if (response.Succeeded)
        {
            _snackBar.Add(_localizer["Password Changed!"], Severity.Success);
            _passwordModel.Password = string.Empty;
            _passwordModel.NewPassword = string.Empty;
            _passwordModel.ConfirmNewPassword = string.Empty;
        }
        else
        {
            if (response.Messages != null)
            {
                foreach (string? message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
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