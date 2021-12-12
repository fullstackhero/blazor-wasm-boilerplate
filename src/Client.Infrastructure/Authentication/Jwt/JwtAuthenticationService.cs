using FSH.BlazorWebAssembly.Shared.Requests.Identity;
using FSH.BlazorWebAssembly.Shared.Response.Identity;
using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.Jwt;

public class JwtAuthenticationService : IAuthenticationService
{
    private readonly NavigationManager _navigationManager;
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly JwtAuthenticationStateProvider _authenticationStateProvider;

    public JwtAuthenticationService(
        HttpClient httpClient,
        ILocalStorageService localStorage,
        JwtAuthenticationStateProvider authenticationStateProvider,
        NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authenticationStateProvider = authenticationStateProvider;
        _navigationManager = navigationManager;
    }

    public AuthProvider ProviderType => AuthProvider.Jwt;

    public async Task<IResult> Login(TokenRequest model)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("tenant", model.Tenant);
        var response = await _httpClient.PostAsJsonAsync(TokenEndpoints.AuthenticationEndpoint, model);
        var result = await response.ToResultAsync<TokenResponse>();
        if (result.Succeeded)
        {
            string? token = result.Data?.Token;
            string? refreshToken = result.Data?.RefreshToken;
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);

            await _authenticationStateProvider.StateChangedAsync();

            // TODO: Shouldn't this be handled by the AuthenticationHeaderHandler?
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await Result.SuccessAsync();
        }
        else
        {
            return await Result.FailAsync(result.Exception);
        }
    }

    public async Task<IResult> Logout()
    {
        await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.ImageUri);
        _authenticationStateProvider.MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        _navigationManager.NavigateTo("/login");
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