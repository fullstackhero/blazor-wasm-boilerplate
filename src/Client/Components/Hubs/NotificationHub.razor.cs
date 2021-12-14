using System.Net;
using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using FSH.BlazorWebAssembly.Shared.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.Hubs;

public partial class NotificationHub
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Inject]
    public IAccessTokenProvider TokenProvider { get; set; } = default!;

    private HubConnection HubConnection { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        string apiBaseUri = _configurations[ConfigConstants.ApiBaseUrl];
        HubConnection = HubConnection!.TryInitialize(TokenProvider, apiBaseUri);
        HubConnection.Reconnecting += Hub_Reconnecting;
        HubConnection.Reconnected += Hub_Reconnected;
        HubConnection.Closed += Hub_Closed;
        HubConnection = await TryConnectAsync();
    }

    public async Task<HubConnection> TryConnectAsync()
    {
        try
        {
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }
        catch (HttpRequestException requestException)
        {
            if (requestException.StatusCode == HttpStatusCode.Unauthorized)
            {
                // The signalR connection is usually the first hit to the actual api after a user logs in with an external Auth Provider (e.g. AzureAd).
                // If a 401 is thrown here, it means the user doesn't have access to the application, so we guide them to a "Not Authorized" page.
                // Sending them back to /login would throw them in an endless loop.
                // In the case of regular jwt auth, this shouldn't happen. If it does, there must be something else wrong...
                _navigationManager.NavigateTo("/notfound");
            }
        }

        return HubConnection;
    }

    private async Task Hub_Closed(Exception? arg)
    {
        _snackBar.Add("SignalR Connection Lost.", Severity.Error, a =>
        {
            a.Icon = Icons.Material.Filled.Error;
            a.RequireInteraction = true;
            a.ShowCloseIcon = true;
        });

        await Task.Delay(5000);

        await TryConnectAsync();
    }

    private Task Hub_Reconnected(string? arg)
    {
        _snackBar.Add("SignalR Connected Restored.", Severity.Success, a =>
        {
            a.Icon = Icons.Material.Filled.CheckCircle;
        });
        return Task.CompletedTask;
    }

    private Task Hub_Reconnecting(Exception? arg)
    {
        _snackBar.Add("SignalR Reconnecting.", Severity.Info, a =>
        {
            a.Icon = Icons.Material.Filled.Refresh;
        });
        return Task.CompletedTask;
    }
}