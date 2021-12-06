using FSH.BlazorWebAssembly.Shared.Response.AuditLogs;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.AuditLogs;

public interface IAuditLogsService : IApiService
{
    Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserAuditLogsAsync();
}