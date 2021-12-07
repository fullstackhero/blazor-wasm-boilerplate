using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Client.Components.ErrorHandler
{
    public partial class ErrorHandler
    {
        public List<Exception> _receivedExceptions = new();

        protected async override Task OnErrorAsync(Exception exception)
        {
            _receivedExceptions.Add(exception);
            switch (exception)
            {
                case UnauthorizedAccessException:
                    await _authService.Logout();
                    _snackBar.Add("Authentication Failed", Severity.Error);
                    _navigationManager.NavigateTo("/login");
                    break;
            }
        }

        public new void Recover()
        {
            _receivedExceptions.Clear();
            base.Recover();
        }
    }
}
