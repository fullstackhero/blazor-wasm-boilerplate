using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication;

public interface IAuthenticationService
{
    AuthProvider ProviderType { get; }
    Task<Result> LoginAsync(string tenantKey, TokenRequest request);
    Task LogoutAsync();
}