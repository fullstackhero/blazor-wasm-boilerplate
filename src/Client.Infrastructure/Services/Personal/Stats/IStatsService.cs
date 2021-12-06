using FSH.BlazorWebAssembly.Shared.Response;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.Stats;

public interface IStatsService : IApiService
{
    Task<IResult<StatsDto>> GetDataAsync();
}