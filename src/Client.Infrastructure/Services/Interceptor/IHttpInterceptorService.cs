using Toolbelt.Blazor;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Interceptor;

public interface IHttpInterceptorService : IApiService
{
    void RegisterEvent();
    Task InterceptResponseAsync(object sender, HttpClientInterceptorEventArgs e);
    void DisposeEvent();
}