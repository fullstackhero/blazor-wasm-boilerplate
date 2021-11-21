using FSH.BlazorWebAssembly.Shared.Response.AuditLogs;
using FSH.BlazorWebAssembly.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.AuditLogs
{
    public interface IAuditLogsService : IApiService
    {
        Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserAuditLogsAsync();
    }
}