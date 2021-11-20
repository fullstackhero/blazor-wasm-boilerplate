using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSH.BlazorWebAssembly.Shared.Response.Logs;
using FSH.BlazorWebAssembly.Shared.Wrapper;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.AuditLogs
{
    public interface IAuditLogsService : IApiService
    {
        Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserAuditLogsAsync();
    }
}