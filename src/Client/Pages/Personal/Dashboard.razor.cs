using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.Stats;
using FSH.BlazorWebAssembly.Shared.Notifications;
using FSH.BlazorWebAssembly.Shared.Notifications.Personal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Client.Pages.Personal
{
    public partial class Dashboard
    {
        [Inject]
        private IStatsService StatsService { get; set; }
        [Parameter]
        public int ProductCount { get; set; }
        [Parameter]
        public int BrandCount { get; set; }

        [Parameter]
        public int UserCount { get; set; }
        [Parameter]
        public int RoleCount { get; set; }

        private bool _loaded = false;

        private HubConnection HubConnection { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_localStorage);
            HubConnection.On<NotificationMessage>("ReceiveMessage", async (notification) =>
            {
                switch(notification.MessageType)
                {
                    case nameof(StatsChangedNotification):
                        await LoadDataAsync();
                        StateHasChanged();
                        break;
                }
            });
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            var response = await StatsService.GetDataAsync();
            if (response.Succeeded)
            {
                ProductCount = response.Data.ProductCount;
                BrandCount = response.Data.BrandCount;
                UserCount = response.Data.UserCount;
                RoleCount = response.Data.RoleCount;
            }
            else
            {
                foreach (string message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }
}
