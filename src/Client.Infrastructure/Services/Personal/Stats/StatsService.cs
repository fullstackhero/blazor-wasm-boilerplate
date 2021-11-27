using FSH.BlazorWebAssembly.Shared.Response;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.Stats
{
    public class StatsService : IStatsService
    {
        private readonly HttpClient _httpClient;

        public StatsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<StatsDto>> GetDataAsync()
        {
            var response = await _httpClient.GetAsync(Shared.Routes.StatsEndpoint.GetData);
            var data = await response.ToResult<StatsDto>();
            return data;
        }
    }
}