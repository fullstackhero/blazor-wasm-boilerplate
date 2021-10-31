using FSH.BlazorWebAssembly.Shared.Requests.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;

namespace FSH.BlazorWebAssembly.Client.Pages.Authentication
{
    public partial class Login
    {

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        void TogglePasswordVisibility()
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

        private void FillAdministratorCredentials()
        {
            tokenRequest.Email = "admin@root.com";
            tokenRequest.Password = "123Pa$$word!";
            tokenRequest.Tenant = "root";
        }
        protected override async Task OnInitializedAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
            {
                _navigationManager.NavigateTo("/");
            }
        }
        private TokenRequest tokenRequest = new();

        private async Task SubmitAsync()
        {
            var result = await _authService.Login(tokenRequest);
            if (!result.Succeeded)
            {
                foreach (var message in result.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}
