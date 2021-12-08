using FSH.BlazorWebAssembly.Shared.Response.AuditLogs;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.AuditLogs;

public class AuditLogsService : IAuditLogsService
{
    private readonly HttpClient _httpClient;

    public AuditLogsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserAuditLogsAsync()
    {
        var response = await _httpClient.GetAsync(AuditLogsEndpoint.GetLogs);
        return await response.ToResultAsync<IEnumerable<AuditResponse>>();
    }
}