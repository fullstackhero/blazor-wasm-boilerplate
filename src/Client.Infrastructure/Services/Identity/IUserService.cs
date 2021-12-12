using FSH.BlazorWebAssembly.Shared.Identity;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Identity;

public interface IUserService : IApiService
{
    Task<IResult<List<PermissionDto>>> GetPermissionsAsync(string userId);
}