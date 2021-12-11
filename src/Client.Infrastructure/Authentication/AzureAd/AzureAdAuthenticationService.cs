using FSH.BlazorWebAssembly.Shared.Requests.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.AzureAd;

internal class AzureAdAuthenticationService : IAuthenticationService
{
    private readonly AuthenticationStateProvider _authState;
    private readonly SignOutSessionStateManager _signOut;
    private readonly NavigationManager _navigation;

    public AzureAdAuthenticationService(AuthenticationStateProvider authState, SignOutSessionStateManager signOut, NavigationManager navigation)
    {
        _authState = authState;
        _signOut = signOut;
        _navigation = navigation;
    }

    public AuthProvider ProviderType => AuthProvider.AzureAd;

    public Task<IResult> Login(TokenRequest model)
    {
        throw new NotImplementedException();
    }

    public async Task<IResult> Logout()
    {
        await _signOut.SetSignOutState();
        _navigation.NavigateTo("authentication/logout");
        return await Result.SuccessAsync();
    }

    public Task<string> RefreshToken()
    {
        throw new NotImplementedException();
    }

    public Task<string> TryForceRefreshToken()
    {
        throw new NotImplementedException();
    }

    public Task<string> TryRefreshToken()
    {
        throw new NotImplementedException();
    }
}