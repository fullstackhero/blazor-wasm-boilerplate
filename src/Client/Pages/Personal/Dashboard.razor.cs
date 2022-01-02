using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Shared.Dashboard;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Personal;

public partial class Dashboard
{
    private readonly string[] _dataEnterBarChartXAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

    private readonly List<MudBlazor.ChartSeries> _dataEnterBarChartSeries = new();

    [Inject]
    private IStatsClient StatsClient { get; set; } = default!;

    [Inject]
    private HubConnection HubConnection { get; set; } = default!;

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
        HubConnection.On<StatsChangedNotification>(nameof(StatsChangedNotification), async _ =>
        {
            await LoadDataAsync();
            StateHasChanged();
        });
    }

    private async Task LoadDataAsync()
    {
        var response = await StatsClient.GetAsync();
        if (response.Succeeded)
        {
            if (response.Data is not null)
            {
                ProductCount = response.Data.ProductCount;
                BrandCount = response.Data.BrandCount;
                UserCount = response.Data.UserCount;
                RoleCount = response.Data.RoleCount;
                foreach (var item in response.Data.DataEnterBarChart)
                {
                    _dataEnterBarChartSeries
                        .RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                    _dataEnterBarChartSeries.Add(new MudBlazor.ChartSeries { Name = item.Name, Data = item.Data?.ToArray() });
                }
            }
        }
        else if (response.Messages is not null)
        {
            foreach (string message in response.Messages)
            {
                Snackbar.Add(message, Severity.Error);
            }
        }
    }
}