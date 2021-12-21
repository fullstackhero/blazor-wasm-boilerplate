using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.AzureAd;

internal class AzureAdAuthenticationService : IAuthenticationService
{
    private readonly SignOutSessionStateManager _signOut;
    private readonly NavigationManager _navigation;

    public AzureAdAuthenticationService(SignOutSessionStateManager signOut, NavigationManager navigation) =>
        (_signOut, _navigation) = (signOut, navigation);

    public AuthProvider ProviderType => AuthProvider.AzureAd;

    public Task<IResult> LoginAsync(string tenantKey, TokenRequest request) =>
        throw new NotImplementedException();

    public async Task<IResult> LogoutAsync()
    {
        await _signOut.SetSignOutState();
        _navigation.NavigateTo("authentication/logout");
        return await Shared.Wrapper.Result.SuccessAsync();
    }

    public Task<IResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request) =>
        throw new NotImplementedException();
}