using System.Net;
using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace FSH.BlazorWebAssembly.Client.Components.Hubs;

public partial class NotificationHub
{
    [CascadingParameter]
    public Error? Error { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    private HubConnection? _hubConnection { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = await TryConnectAsync().ConfigureAwait(true);
    }

    public async Task<HubConnection> TryConnectAsync()
    {
        string apiBaseUri = _configurations.GetValue<string>("FullStackHero.API");
        _hubConnection = _hubConnection!.TryInitialize(_localStorage, apiBaseUri);
        _hubConnection.Closed += Hub_Closed;
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
                _snackBar.Add("SingalR Client Unauthorized.", MudBlazor.Severity.Error);
                await _authService.Logout();
            }
        }

        return _hubConnection;
    }

    private async Task Hub_Closed(Exception? arg)
    {
        _snackBar.Add("SingalR Connection Closed.", MudBlazor.Severity.Error, a =>
        {
            a.RequireInteraction = true;
            a.ShowCloseIcon = true;
        });

        await Task.CompletedTask;
    }
}