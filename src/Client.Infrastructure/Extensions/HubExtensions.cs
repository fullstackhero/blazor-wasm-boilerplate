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
                        () => tokenProvider.GetAccessTokenAsync())
                .WithAutomaticReconnect()
                .Build();
        }

        return hubConnection;
    }
}