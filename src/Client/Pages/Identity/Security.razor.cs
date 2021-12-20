using System.Security.Claims;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
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

    private readonly ResetPasswordRequest _passwordModel = new();
    private string? ConfirmationPassword { get; set; }

    private async Task ChangePasswordAsync()
    {
        if (_passwordModel.Password?.Equals(ConfirmationPassword) ?? false)
        {
            var authState = await AuthState;
            _passwordModel.Email = authState.User.FindFirstValue(ClaimTypes.Email);

            // TODO: this token needs to be a generated token sent via mail if i'm not mistaken...
            _passwordModel.Token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            var response = await IdentityClient.ResetPasswordAsync(_passwordModel);
            if (response.Succeeded)
            {
                _snackBar.Add(_localizer["Password Changed!"], Severity.Success);
                _passwordModel.Password = string.Empty;
                _passwordModel.Email = string.Empty;
                _passwordModel.Token = string.Empty;
            }
            else
            {
                if (response.Messages?.Count > 0)
                {
                    foreach (string message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }
        else
        {
            _snackBar.Add(_localizer["Confirmation Password Must Be Same"], Severity.Info);
        }
    }

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