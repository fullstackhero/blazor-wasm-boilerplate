using Toolbelt.Blazor;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.Jwt;

public interface IHttpInterceptorService : IApiService
{
    void RegisterEvent();
    Task InterceptResponseAsync(object sender, HttpClientInterceptorEventArgs e);
    void DisposeEvent();
}