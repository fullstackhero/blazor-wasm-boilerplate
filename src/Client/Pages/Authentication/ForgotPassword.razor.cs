using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityHunter.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using CityHunter.BlazorWebAssembly.Client.Shared;


namespace CityHunter.BlazorWebAssembly.Client.Pages.Authentication;
public partial class ForgotPassword
{
    public ForgotPassword()
    {

    }

    private ForgotPasswordRequest _forgotPasswordRequest = new ForgotPasswordRequest();
    public bool BusySubmitting { get; set; } = false;

    private CustomValidation? _customValidation;

    [Inject]
    public IIdentityClient _identityClient { get; set; } = default!;

    public string _tenant { get; set; } = "root";
    private async Task SubmitAsync()
    {
        BusySubmitting = true;
        // _tenant = "root";
        await ApiHelper.ExecuteCallGuardedAsync(
            () => _identityClient.ForgotPasswordAsync(_tenant, _forgotPasswordRequest),
            _snackBar,
            _customValidation);


        BusySubmitting = false;
    }
}
