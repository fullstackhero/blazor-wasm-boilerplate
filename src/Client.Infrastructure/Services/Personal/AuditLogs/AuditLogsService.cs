using FSH.BlazorWebAssembly.Shared.Response.AuditLogs;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.AuditLogs
{
    public class AuditLogsService : IAuditLogsService
    {
        private readonly HttpClient _httpClient;

        public AuditLogsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserAuditLogsAsync()
        {
            var response = await _httpClient.GetAsync(Shared.Routes.AuditLogsEndpoint.GetLogs);
            var data = await response.ToResult<IEnumerable<AuditResponse>>();
            return data;
        }
    }
}