using FSH.BlazorWebAssembly.Shared.Identity;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Identity.Account;
public interface IAccountManager : IApiService
{
    Task<IResult> ChangePasswordAsync(ResetPasswordRequest model);

}