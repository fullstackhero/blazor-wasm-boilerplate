using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Client.Components.Hubs
{
    public partial class NotificationHub
    {
        [CascadingParameter]
        public Error Error { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; } = new RenderFragment(x => { });

        public HubConnection HubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            HubConnection = await TryConnectAsync().ConfigureAwait(true);
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