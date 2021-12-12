using FSH.BlazorWebAssembly.Client.Infrastructure.Services.Identity;
using FSH.BlazorWebAssembly.Shared.Identity;
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
            var profileResult = await _serviceProvider.GetRequiredService<IIdentityService>()
                .GetProfileDetailsAsync();

            if (profileResult.Succeeded && profileResult.Data is UserDetailsDto userDetails)
            {
                var userIdentity = (ClaimsIdentity)principal.Identity;

                if (!userIdentity.HasClaim(c => c.Type == ClaimTypes.Email) && userDetails.Email is not null)
                {
                    userIdentity.AddClaim(new Claim(ClaimTypes.Email, userDetails.Email));
                }

                if (!userIdentity.HasClaim(c => c.Type == ClaimTypes.MobilePhone) && userDetails.PhoneNumber is not null)
                {
                    userIdentity.AddClaim(new Claim(ClaimTypes.MobilePhone, userDetails.PhoneNumber));
                }

                if (!userIdentity.HasClaim(c => c.Type == "fullname"))
                {
                    userIdentity.AddClaim(new Claim("fullname", $"{userDetails.FirstName} {userDetails.LastName}"));
                }

                if (!userIdentity.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    userIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userDetails.Id.ToString()));
                }

                if (!userIdentity.HasClaim(c => c.Type == "image_url") && userDetails.ImageUrl is not null)
                {
                    userIdentity.AddClaim(new Claim("image_url", userDetails.ImageUrl));
                }

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