using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FSH.BlazorWebAssembly.Client.Shared;

public partial class PersonCard
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    private string? UserId { get; set; }
    private string? Email { get; set; }
    private string? FullName { get; set; }
    private string? ImageUri { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadUserData();
        }
    }

    private async Task LoadUserData()
    {
        // This should load its data from the userdata cache that's hydrated
        // when the user logged in (Authentication.OnLoginSucceeded for AzureAd)
        var authState = await AuthState;
        var user = authState.User;
        if (user == null) return;
        if (user.Identity?.IsAuthenticated == true)
        {
            if (string.IsNullOrEmpty(UserId))
            {
                FullName = user.FindFirstValue("name") ?? user.GetName();
                UserId = user.GetUserId();
                Email = user.FindFirstValue("preferred_username") ?? user.GetEmail();
                StateHasChanged();
            }
        }
    }
}