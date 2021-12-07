using Microsoft.AspNetCore.SignalR.Client;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;

public static class HubExtensions
{
    public static HubConnection TryInitialize(this HubConnection hubConnection, ILocalStorageService localStorage, string apiBaseUri)
    {
        if (hubConnection == null)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{apiBaseUri}notifications", options =>
                    options.AccessTokenProvider =
                        async () => await localStorage.GetItemAsync<string>("authToken"))
                .Build();
        }

        return hubConnection;
    }
}