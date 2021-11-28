using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using FSH.BlazorWebAssembly.Shared.Identity;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Identity.Account;
public class AccountManager : IAccountManager
{
    private readonly HttpClient _httpClient;

    public AccountManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult> ChangePasswordAsync(ResetPasswordRequest model)
    {
        var response = await _httpClient.PostAsJsonAsync(AccountEndpoints.ChangePassword, model);
        return await response.ToResult();
    }

}