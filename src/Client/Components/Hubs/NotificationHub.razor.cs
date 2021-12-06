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

    public HubConnection? HubConnection { get; set; }

    protected override async Task OnInitializedAsync()
    {
        HubConnection = await TryConnectAsync().ConfigureAwait(true);
    }

    public async Task<HubConnection> TryConnectAsync()
    {
        string apiBaseUri = _configurations.GetValue<string>("FullStackHero.API");
        HubConnection = HubConnection!.TryInitialize(_localStorage, apiBaseUri);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }

        return HubConnection;
    }
}