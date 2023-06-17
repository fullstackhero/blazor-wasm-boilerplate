using FL_CRMS_ERP_WASM.Client.Components.Common;
using FL_CRMS_ERP_WASM.Client.Infrastructure.ApiClient;
using FL_CRMS_ERP_WASM.Client.Shared;
using FSH.WebApi.Shared.Multitenancy;
using Microsoft.AspNetCore.Components;

namespace FL_CRMS_ERP_WASM.Client.Pages.Authentication;

public partial class ForgotPassword
{
    private readonly ForgotPasswordRequest _forgotPasswordRequest = new();
    private CustomValidation? _customValidation;
    private bool BusySubmitting { get; set; }

    [Inject]
    private IUsersClient UsersClient { get; set; } = default!;

    private string Tenant { get; set; } = MultitenancyConstants.Root.Id;

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.ForgotPasswordAsync(Tenant, _forgotPasswordRequest),
            Snackbar,
            _customValidation);

        BusySubmitting = false;
    }
}