using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;

public static class HubExtensions
{
    public static HubConnection TryInitialize(this HubConnection hubConnection, IAccessTokenProvider tokenProvider, string apiBaseUri)
    {
        if (hubConnection == null)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{apiBaseUri}notifications", options =>
                    options.AccessTokenProvider =
                        () => GetAccessTokenAsync(tokenProvider))
                .WithAutomaticReconnect()
                .Build();
        }

        return hubConnection;
    }

    private static async Task<string?> GetAccessTokenAsync(IAccessTokenProvider tokenProvider) =>
        (await tokenProvider.RequestAccessToken())
            .TryGetToken(out var token)
                ? token.Value
                : null;
}