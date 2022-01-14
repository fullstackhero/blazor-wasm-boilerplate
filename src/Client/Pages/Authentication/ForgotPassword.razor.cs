﻿using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.BlazorWebAssembly.Shared.MultiTenancy;
using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Pages.Authentication;

public partial class ForgotPassword
{
    private readonly ForgotPasswordRequest _forgotPasswordRequest = new();
    private CustomValidation? _customValidation;
    private bool BusySubmitting { get; set; }

    [Inject]
    private IIdentityClient IdentityClient { get; set; } = default!;

    private string Tenant { get; set; } = MultitenancyConstants.DefaultTenant.Key;

    private async Task SubmitAsync()
    {
        BusySubmitting = true;

        await ApiHelper.ExecuteCallGuardedAsync(
            () => IdentityClient.ForgotPasswordAsync(Tenant, _forgotPasswordRequest),
            Snackbar,
            _customValidation);

        BusySubmitting = false;
    }
}