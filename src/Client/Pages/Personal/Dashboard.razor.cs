using System.Threading.Tasks;
using FSH.BlazorWebAssembly.Client.Components.Hubs;
using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.Stats;
using FSH.BlazorWebAssembly.Shared.Notifications;
using FSH.BlazorWebAssembly.Shared.Notifications.Personal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Personal
{
    public partial class Dashboard
    {
        [Inject]
        private IStatsService StatsService { get; set; }
        [CascadingParameter]
        public NotificationHub NotificationHub { get; set; }
        [Parameter]
        public int ProductCount { get; set; }
        [Parameter]
        public int BrandCount { get; set; }

        [Parameter]
        public int UserCount { get; set; }
        [Parameter]
        public int RoleCount { get; set; }

        private bool _loaded = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            _loaded = true;
            NotificationHub.TryConnectAsync().Result.On<StatsChangedNotification>(nameof(StatsChangedNotification), async (notification) =>
            {
                await LoadDataAsync();
                StateHasChanged();
            });
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