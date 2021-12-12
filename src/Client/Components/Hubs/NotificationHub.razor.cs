using System.Net;
using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
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

    private HubConnection? _hubConnection { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = await TryConnectAsync();
    }

    public async Task<HubConnection> TryConnectAsync()
    {
        string apiBaseUri = _configurations.GetValue<string>("ApiUrl");
        _hubConnection = _hubConnection!.TryInitialize(TokenProvider, apiBaseUri);
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

                _navigationManager.NavigateTo("/login");
            }
        }

        return _hubConnection;
    }

    private Task Hub_Closed(Exception? arg)
    {
        _snackBar.Add("SingalR Connection Closed.", MudBlazor.Severity.Error, a =>
        {
            a.RequireInteraction = true;
            a.ShowCloseIcon = true;
        });

        return Task.CompletedTask;
    }
}