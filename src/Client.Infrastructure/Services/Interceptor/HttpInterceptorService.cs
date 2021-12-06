using System.Net;
using FSH.BlazorWebAssembly.Client.Infrastructure.Identity.Authentication;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Toolbelt.Blazor;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Services.Interceptor;

public class HttpInterceptorService : IHttpInterceptorService
{
    private readonly HttpClientInterceptor _interceptor;
    private readonly NavigationManager _navManager;
    private readonly IAuthenticationService _authenticationService;
    private readonly ISnackbar _snackBar;
    public HttpInterceptorService(HttpClientInterceptor interceptor, NavigationManager navManager, IAuthenticationService authenticationService, ISnackbar snackBar)
    {
        _interceptor = interceptor;
        _navManager = navManager;
        _authenticationService = authenticationService;
        _snackBar = snackBar;
    }

    public void RegisterEvent() => _interceptor.AfterSendAsync += InterceptResponseAsync;
    public async Task InterceptResponseAsync(object sender, HttpClientInterceptorEventArgs e)
    {
        string message = string.Empty;
        if (!e.Response.IsSuccessStatusCode)
        {
            var statusCode = e.Response.StatusCode;
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    break;
                case HttpStatusCode.Unauthorized:
                    await _authenticationService.Logout();
                    _snackBar.Add("Authentication Failed", Severity.Error);
                    _navManager.NavigateTo("/login");
                    break;
                default:
                    break;
            }
        }
    }

    public void DisposeEvent() => _interceptor.AfterSendAsync -= InterceptResponseAsync;
}