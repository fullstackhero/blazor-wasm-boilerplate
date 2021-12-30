using Microsoft.AspNetCore.Components;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;

namespace FSH.BlazorWebAssembly.Client.Pages.Authentication;
public partial class ForgotPassword
{
    private ForgotPasswordRequest _forgotPasswordRequest = new ForgotPasswordRequest();
    public bool BusySubmitting { get; set; } = false;

    private CustomValidation? _customValidation;

    [Inject]
    public IIdentityClient _identityClient { get; set; } = default!;

    public string _tenant { get; set; } = "root";
    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        await ApiHelper.ExecuteCallGuardedAsync(
            () => _identityClient.ForgotPasswordAsync(_tenant, _forgotPasswordRequest),
            _snackBar,
            _customValidation);

        BusySubmitting = false;
    }
}
