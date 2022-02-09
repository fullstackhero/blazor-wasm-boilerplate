using System.Net;
using FSH.BlazorWebAssembly.Client.Infrastructure.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Notifications;

public class NotificationClient : IAsyncDisposable
{
    private readonly IConfiguration _config;
    private readonly NavigationManager _navigation;
    private readonly IAuthenticationService _authService;
    private readonly CancellationTokenSource _cts = new();

    public HubConnection HubConnection { get; private set; }

    public ConnectionState ConnectionState =>
        HubConnection.State switch
        {
            HubConnectionState.Connected => ConnectionState.Connected,
            HubConnectionState.Disconnected => ConnectionState.Disconnected,
            _ => ConnectionState.Connecting
        };

    public event EventHandler<ConnectionStateChangedEventArgs>? ConnectionStateChanged;

    public NotificationClient(IAccessTokenProvider tokenProvider, IConfiguration config, NavigationManager navigation, IAuthenticationService authService)
    {
        _config = config;
        _navigation = navigation;
        _authService = authService;

        HubConnection = new HubConnectionBuilder()
            .WithUrl($"{config[ConfigNames.ApiBaseUrl]}notifications", options =>
                options.AccessTokenProvider =
                    () => tokenProvider.GetAccessTokenAsync())
            .WithAutomaticReconnect()
            .Build();

        HubConnection.Reconnecting += ex =>
            OnConnectionStateChangedAsync(ConnectionState.Connecting, ex?.Message);

        HubConnection.Reconnected += id =>
            OnConnectionStateChangedAsync(ConnectionState.Connected, id);

        HubConnection.Closed += async ex =>
        {
            await OnConnectionStateChangedAsync(ConnectionState.Disconnected, ex?.Message);

            // Once initialized the retry logic configured in the HubConnection will automatically attempt to reconnect
            // However, once it reaches its maximum number of attempts, it will give up and needs to be manually started again
            // handling this event we can manually attempt to reconnect
            await ConnectWithRetryAsync(_cts.Token);
        };
    }

    public Task TryConnectAsync() =>
        ConnectWithRetryAsync(_cts.Token);

    protected virtual Task OnConnectionStateChangedAsync(ConnectionState state, string? message)
    {
        ConnectionStateChanged?.Invoke(this, new(state, message));
        return Task.CompletedTask;
    }

    private async Task ConnectWithRetryAsync(CancellationToken cancellationToken)
    {
        // Keep trying to until we can start or the token is canceled.
        while (true)
        {
            try
            {
                await HubConnection.StartAsync(cancellationToken);
                await OnConnectionStateChangedAsync(ConnectionState.Connected, HubConnection.ConnectionId);
            }
            catch when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (HttpRequestException requestException) when (requestException.StatusCode == HttpStatusCode.Unauthorized)
            {
                // The signalR connection is usually the first hit to the actual api after a user logs in with an external Auth Provider (e.g. AzureAd).
                // If a 401 is thrown here, it means the user doesn't have access to the application, so we guide them to a "Not Found" page.
                // Sending them back to /login would throw them in an endless loop.
                // In the case of regular jwt auth, this shouldn't happen. If it does, there must be something else wrong...
                switch (_config[nameof(AuthProvider)])
                {
                    case nameof(AuthProvider.AzureAd):
                        _navigation.NavigateTo("/notfound");
                        break;

                    default:
                        await _authService.LogoutAsync();
                        break;
                }

                return;
            }
            catch
            {
                // Try again in a few seconds. This could be an incremental interval
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _cts.Dispose();
        await HubConnection.DisposeAsync();
    }
}