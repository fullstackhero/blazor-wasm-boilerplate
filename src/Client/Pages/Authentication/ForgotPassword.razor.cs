using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Pages.Authentication;

public partial class ForgotPassword
{
    private readonly ForgotPasswordRequest _forgotPasswordRequest = new();
    private CustomValidation? _customValidation;
    public bool BusySubmitting { get; set; }

    [Inject] public IIdentityClient _identityClient { get; set; } = default!;

    public string _tenant { get; set; } = string.Empty;

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