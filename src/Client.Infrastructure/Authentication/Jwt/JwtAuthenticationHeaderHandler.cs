using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.Jwt;

public class JwtAuthenticationHeaderHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _tokenProvider;

    public JwtAuthenticationHeaderHandler(IAccessTokenProvider tokenProvider) =>
        _tokenProvider = tokenProvider;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // skip token endpoints
        if (request.RequestUri?.AbsolutePath.Contains("/tokens") == false)
        {
            request.Headers.Authorization =
                await _tokenProvider.GetAccessTokenAsync() is string token
                    ? new AuthenticationHeaderValue("Bearer", token)
                    : null;
        }

        return await base.SendAsync(request, cancellationToken);
    }
}