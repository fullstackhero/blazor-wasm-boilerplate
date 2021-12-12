using FSH.BlazorWebAssembly.Client.Infrastructure.Authentication;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.ErrorHandler;

public partial class ErrorHandler
{
    [Inject]
    public IAuthenticationService AuthService { get; set; } = default!;

    public List<Exception> _receivedExceptions = new();

    protected async override Task OnErrorAsync(Exception exception)
    {
        _receivedExceptions.Add(exception);
        switch (exception)
        {
            case UnauthorizedAccessException:
                await AuthService.Logout();
                _snackBar.Add("Authentication Failed", Severity.Error);
                break;
        }
    }

    public new void Recover()
    {
        _receivedExceptions.Clear();
        base.Recover();
    }
}
