using Blazored.LocalStorage;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Authentication;
using FSH.BlazorWebAssembly.Shared.Constants;
using FSH.BlazorWebAssembly.Shared.Requests.Identity;
using FSH.BlazorWebAssembly.Shared.Response.Identity;
using FSH.BlazorWebAssembly.Shared.Routes;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Identity.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationService(
            HttpClient httpClient,
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }
        public async Task<ClaimsPrincipal> CurrentUser()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User;
        }

        public async Task<IResult> Login(TokenRequest model)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("tenantKey", model.Tenant);
            var response = await _httpClient.PostAsJsonAsync(TokenEndpoints.AuthenticationEndpoint, model);
            var result = await response.ToResult<TokenResponse>();
            if (result.Succeeded)
            {
                var token = result.Data.Token;
                var refreshToken = result.Data.RefreshToken;
                await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
                await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);

                await ((ApplicationAuthenticationStateProvider)this._authenticationStateProvider).StateChangedAsync();

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
            ((ApplicationAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            _httpClient.DefaultRequestHeaders.Authorization = null;
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
}
