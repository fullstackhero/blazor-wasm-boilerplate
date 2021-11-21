using FSH.BlazorWebAssembly.Shared.Response;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.Stats
{
    public interface IStatsService : IApiService
    {
        Task<IResult<StatsDto>> GetDataAsync();
    }
}
