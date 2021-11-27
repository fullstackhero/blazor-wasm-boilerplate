using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Extensions
{
    public static class HubExtensions
    {
        public static HubConnection TryInitialize(this HubConnection hubConnection, ILocalStorageService localStorage)
        {
            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder()
                                  .WithUrl("https://localhost:5001/notifications", options =>
                                  {

                                      options.AccessTokenProvider = async () => (await localStorage.GetItemAsync<string>("authToken"));
                                  })
                                  .WithAutomaticReconnect()
                                  .Build();
            }

            return hubConnection;
        }
    }
}