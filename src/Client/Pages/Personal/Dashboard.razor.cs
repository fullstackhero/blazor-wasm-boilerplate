using FSH.BlazorWebAssembly.Client.Components.Hubs;
using FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.Stats;
using FSH.BlazorWebAssembly.Shared.Notifications.Personal;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Personal;

public partial class Dashboard
{
    [Inject]
    private IStatsService StatsService { get; set; } = default!;

    [CascadingParameter]
    public NotificationHub? NotificationHub { get; set; }

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
        var hubConnection = await NotificationHub!.TryConnectAsync();
        hubConnection.On<StatsChangedNotification>(nameof(StatsChangedNotification), async _ =>
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
            if (response.Data is not null)
            {
                ProductCount = response.Data.ProductCount;
                BrandCount = response.Data.BrandCount;
                UserCount = response.Data.UserCount;
                RoleCount = response.Data.RoleCount;
            }
        }
        else if (response.Messages is not null)
        {
            foreach (string message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
}