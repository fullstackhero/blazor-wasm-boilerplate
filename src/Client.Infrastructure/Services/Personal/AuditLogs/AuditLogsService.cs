using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSH.BlazorWebAssembly.Shared.Response.Logs;
using FSH.BlazorWebAssembly.Shared.Wrapper;

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