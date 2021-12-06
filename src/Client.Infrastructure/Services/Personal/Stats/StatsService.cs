using FSH.BlazorWebAssembly.Shared.Response;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.Stats;

public class StatsService : IStatsService
{
    private readonly HttpClient _httpClient;

    public StatsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult<StatsDto>> GetDataAsync()
    {
        var response = await _httpClient.GetAsync(StatsEndpoint.GetData);
        var data = await response.ToResultAsync<StatsDto>();
        return data;
    }
}