using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Auth.AzureAd;

public class AzureAdAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public AzureAdAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation, IConfiguration config)
        : base(provider, navigation) => ConfigureHandler(
            new[] { config[ConfigNames.ApiBaseUrl] },
            new[] { config[$"{nameof(AuthProvider.AzureAd)}:{ConfigNames.ApiScope}"] });
}