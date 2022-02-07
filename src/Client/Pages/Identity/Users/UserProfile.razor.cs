using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Auth;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity.Users;

public partial class UserProfile
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    protected IUsersClient UsersClient { get; set; } = default!;

    [Parameter]
    public string? Id { get; set; }
    [Parameter]
    public string? Title { get; set; }
    [Parameter]
    public string? Description { get; set; }

    private bool _active;
    private char _firstLetterOfName;
    private string? _firstName;
    private string? _lastName;
    private string? _phoneNumber;
    private string? _email;
    private string? _imageUrl;
    private bool _loaded;
    private bool _canToggleUserStatus;

    private async Task ToggleUserStatus()
    {
        var request = new ToggleUserStatusRequest { ActivateUser = _active, UserId = Id };
        await ApiHelper.ExecuteCallGuardedAsync(() => UsersClient.ToggleStatusAsync(Id, request), Snackbar);
        Navigation.NavigateTo("/users");
    }

    [Parameter]
    public string? ImageUrl { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => UsersClient.GetByIdAsync(Id), Snackbar)
            is UserDetailsDto user)
        {
            _firstName = user.FirstName;
            _lastName = user.LastName;
            _email = user.Email;
            _phoneNumber = user.PhoneNumber;
            _active = user.IsActive;
            _imageUrl = string.IsNullOrEmpty(user.ImageUrl) ? string.Empty : (Config[ConfigNames.ApiBaseUrl] + user.ImageUrl);

            Title = $"{_firstName} {_lastName}'s {_localizer["Profile"]}";
            Description = _email;
            if (_firstName?.Length > 0)
            {
                _firstLetterOfName = _firstName.ToUpper().FirstOrDefault();
            }
        }

        var state = await AuthState;
        _canToggleUserStatus = await AuthService.HasPermissionAsync(state.User, FSHAction.Update, FSHResource.Users);
        _loaded = true;
    }
}