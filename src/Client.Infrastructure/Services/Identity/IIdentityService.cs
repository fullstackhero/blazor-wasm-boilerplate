using FSH.BlazorWebAssembly.Shared.Identity;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Identity;

public interface IIdentityService : IApiService
{
    Task<IResult<UserDetailsDto>> GetProfileDetailsAsync();
    Task<IResult> ResetPasswordAsync(ResetPasswordRequest model);
}