using FSH.BlazorWebAssembly.Client.Infrastructure.Services.Identity;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.AzureAd;

internal class AzureAdClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
    // can't work with actual services in the constructor here, have to
    // use IServiceProvider, otherwise the app hangs at startup
    private readonly IServiceProvider _serviceProvider;

    public AzureAdClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor, IServiceProvider serviceProvider)
        : base(accessor) =>
        _serviceProvider = serviceProvider;

    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account, RemoteAuthenticationUserOptions options)
    {
        var principal = await base.CreateUserAsync(account, options);

        if (principal.Identity?.IsAuthenticated is true)
        {
            var userIdentity = (ClaimsIdentity)principal.Identity;
            var profileResult = await _serviceProvider.GetRequiredService<IIdentityService>()
                .GetProfileDetailsAsync();
            if (profileResult.Succeeded && profileResult.Data?.Id is not null)
            {
                var permissionsResult = await _serviceProvider.GetRequiredService<IUserService>()
                    .GetPermissionsAsync(profileResult.Data.Id.ToString());
                if (permissionsResult.Succeeded && permissionsResult.Data is not null)
                {
                    userIdentity.AddClaims(permissionsResult.Data
                        .Where(p => !string.IsNullOrWhiteSpace(p.Permission))
                        .Select(p => new Claim("Permission", p.Permission!)));
                }
            }
        }

        return principal;
    }
}
