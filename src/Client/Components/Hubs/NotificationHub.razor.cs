using System.Threading.Tasks;
using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace FSH.BlazorWebAssembly.Client.Components.Hubs
{
    public partial class NotificationHub
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; } = new RenderFragment(x => { });

        public HubConnection HubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            HubConnection = await TryConnectAsync();
        }

        public async Task<HubConnection> TryConnectAsync()
        {
            HubConnection = HubConnection.TryInitialize(_localStorage);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }

            return HubConnection;
        }
    }
}