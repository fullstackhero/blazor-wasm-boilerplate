using System.Net;
using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using FSH.BlazorWebAssembly.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;

namespace FSH.BlazorWebAssembly.Client.Components.Hubs;

public partial class NotificationHub
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Inject]
    public IAccessTokenProvider TokenProvider { get; set; } = default!;

    private HubConnection _hubConnection { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        string apiBaseUri = _configurations[ConfigConstants.ApiBaseUrl];
        _hubConnection = _hubConnection!.TryInitialize(TokenProvider, apiBaseUri);
        _hubConnection.Reconnecting += Hub_Reconnecting;
        _hubConnection.Reconnected += Hub_Reconnected;
        _hubConnection.Closed += Hub_Closed;
        _hubConnection = await TryConnectAsync();
    }

    public async Task<HubConnection> TryConnectAsync()
    {
        try
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }
        catch (HttpRequestException requestException)
        {
            if (requestException.StatusCode == HttpStatusCode.Unauthorized)
            {
                // The signalR connections is usually the first hit to the actual api after a user logs in with an external Auth Provider (e.g. AzureAd).
                // If a 401 is thrown here, it means the user doesn't have access to the application, so we guide them to a "Not Authorized" page.
                // Sending them back to /login would throw them in an endless loop.
                // In the case of regular jwt auth, this shouldn't happen. If it does, there must be something else wrong...
                _navigationManager.NavigateTo("/notfound");
            }
        }

        return _hubConnection;
    }

    private async Task Hub_Closed(Exception? arg)
    {
        _snackBar.Add($"SignalR Connection Closed ({arg?.Message}). Will try to reconnect in a bit.", MudBlazor.Severity.Error, a =>
        {
            a.RequireInteraction = true;
            a.ShowCloseIcon = true;
        });

        await Task.Delay(5000);

        await TryConnectAsync();
    }

    private Task Hub_Reconnected(string? arg)
    {
        _snackBar.Add($"SignalR Connection Reconnected ({arg}).", MudBlazor.Severity.Error);
        return Task.CompletedTask;
    }

    private Task Hub_Reconnecting(Exception? arg)
    {
        _snackBar.Add($"SignalR Connection Reconnecting ({arg?.Message}).", MudBlazor.Severity.Error);
        return Task.CompletedTask;
    }
}