using FSH.BlazorWebAssembly.Shared.Identity;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient) =>
        _httpClient = httpClient;

    public async Task<IResult<List<PermissionDto>>> GetPermissionsAsync(string userId)
    {
        var response = await _httpClient.GetAsync(UsersEndpoints.GetPermissions(userId));
        return await response.ToResultAsync<List<PermissionDto>>();
    }
}
