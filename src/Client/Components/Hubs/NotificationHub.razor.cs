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
                _snackBar.Add("SingalR Client Unauthorized.", MudBlazor.Severity.Error);

                _navigationManager.NavigateTo("/login");
            }
        }

        return HubConnection;
    }

    private async Task Hub_Closed(Exception? arg)
    {
        _snackBar.Add("SignalR Connection Lost.", MudBlazor.Severity.Error, a =>
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
        _snackBar.Add("SignalR Connected Restored.", MudBlazor.Severity.Success, a =>
        {
            a.Icon = Icons.Material.Filled.CheckCircle;
        });
        return Task.CompletedTask;
    }

    private Task Hub_Reconnecting(Exception? arg)
    {
        _snackBar.Add("SignalR Reconnecting.", MudBlazor.Severity.Info, a =>
        {
            a.Icon = Icons.Material.Filled.Refresh;
        });
        return Task.CompletedTask;
    }
}