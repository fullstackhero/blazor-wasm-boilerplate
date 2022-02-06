using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.WebApi.Shared.Notifications;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace FSH.BlazorWebAssembly.Client.Pages.Personal;

public partial class Dashboard
{
    [Parameter]
    public int ProductCount { get; set; }
    [Parameter]
    public int BrandCount { get; set; }
    [Parameter]
    public int UserCount { get; set; }
    [Parameter]
    public int RoleCount { get; set; }

    [Inject]
    private IDashboardClient DashboardClient { get; set; } = default!;
    [Inject]
    private HubConnection HubConnection { get; set; } = default!;

    private readonly string[] _dataEnterBarChartXAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
    private readonly List<MudBlazor.ChartSeries> _dataEnterBarChartSeries = new();
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        HubConnection.On<StatsChangedNotification>(nameof(StatsChangedNotification), async _ =>
        {
            await LoadDataAsync();
            StateHasChanged();
        });

        await LoadDataAsync();

        _loaded = true;
    }

    private async Task LoadDataAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => DashboardClient.GetAsync(),
                Snackbar)
            is StatsDto statsDto)
        {
            ProductCount = statsDto.ProductCount;
            BrandCount = statsDto.BrandCount;
            UserCount = statsDto.UserCount;
            RoleCount = statsDto.RoleCount;
            foreach (var item in statsDto.DataEnterBarChart)
            {
                _dataEnterBarChartSeries
                    .RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                _dataEnterBarChartSeries.Add(new MudBlazor.ChartSeries { Name = item.Name, Data = item.Data?.ToArray() });
            }
        }
    }
}