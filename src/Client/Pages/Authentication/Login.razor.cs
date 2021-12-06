using System.Security.Claims;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.BlazorWebAssembly.Shared.Requests.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Authentication;

public partial class Login
{
    [CascadingParameter]
    public Error? Error { get; set; }

    public bool BusySubmitting { get; set; } = false;
    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

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

    private void FillAdministratorCredentials()
    {
        _tokenRequest.Email = "admin@root.com";
        _tokenRequest.Password = "123Pa$$word!";
        _tokenRequest.Tenant = "root";
    }

    protected override async Task OnInitializedAsync()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
        {
            _navigationManager.NavigateTo("/");
        }
    }

    private readonly TokenRequest _tokenRequest = new();

    private async Task SubmitAsync()
    {
        try
        {
            BusySubmitting = true;
            var result = await _authService.Login(_tokenRequest);
            if (!result.Succeeded && result.Messages is not null)
            {
                Error?.ProcessError(result.Messages);
            }
        }
        catch (Exception ex)
        {
            Error?.ProcessError(ex);
        }
        finally
        {
            BusySubmitting = false;
        }
    }
}