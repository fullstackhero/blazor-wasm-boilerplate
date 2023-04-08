using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Auth.Jwt;

public class JwtAuthenticationHeaderHandler : DelegatingHandler
{
    private readonly IAccessTokenProviderAccessor _tokenProviderAccessor;
    private readonly NavigationManager _navigation;

    public JwtAuthenticationHeaderHandler(IAccessTokenProviderAccessor tokenProviderAccessor, NavigationManager navigation)
    {
        _tokenProviderAccessor = tokenProviderAccessor;
        _navigation = navigation;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // skip token endpoints and pages with attribute [AllowAnonymous]
        if (request.RequestUri?.AbsolutePath.Contains("/tokens") is not true &&
          request.RequestUri?.AbsolutePath.Contains("/self-register") is not true &&
          request.RequestUri?.AbsolutePath.Contains("/forgot-password") is not true)
        {
            if (await _tokenProviderAccessor.TokenProvider.GetAccessTokenAsync() is string token)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _navigation.NavigateTo("/login");
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
