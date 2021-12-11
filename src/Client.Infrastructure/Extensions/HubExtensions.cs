using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;

public static class HubExtensions
{
    public static HubConnection TryInitialize(this HubConnection hubConnection, ILocalStorageService localStorage, IAccessTokenProvider tokenProvider, string apiBaseUri)
    {
        if (hubConnection == null)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{apiBaseUri}notifications", options =>
                    options.AccessTokenProvider =
                        () => GetAccessTokenAsync(localStorage, tokenProvider))
                .WithAutomaticReconnect()
                .Build();
        }

        return hubConnection;
    }

    private static async Task<string?> GetAccessTokenAsync(ILocalStorageService localStorage, IAccessTokenProvider tokenProvider)
    {
        var request = await tokenProvider.RequestAccessToken();
        if (request.TryGetToken(out var token))
        {
            return token.Value;
        }

        return await localStorage.GetItemAsync<string>("authToken");
    }
}