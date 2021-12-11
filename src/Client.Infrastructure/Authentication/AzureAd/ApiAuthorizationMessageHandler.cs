using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.AzureAd;

public class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ApiAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation, IConfiguration config)
        : base(provider, navigation)
    {
        ConfigureHandler(
            new[] { config["ApiUrl"] },
            new[] { config[$"{nameof(AuthProvider.AzureAd)}:ApiScope"] });
    }
}
