using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity;

public partial class Users
{
    [Inject]
    private IUsersClient _usersClient { get; set; } = default!;
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    private IEnumerable<UserDetailsDto>? _userList;
    private UserDetailsDto _user = new();
    private string _searchString = string.Empty;
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;

    private ClaimsPrincipal _currentUser;
    private bool _canCreateUsers;
    private bool _canSearchUsers;
    private bool _canExportUsers;
    private bool _canViewRoles;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _currentUser = state.User;
        _canCreateUsers = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Users.Create)).Succeeded;
        _canSearchUsers = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Users.Search)).Succeeded;
        _canExportUsers = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Users.Export)).Succeeded;
        _canViewRoles = (await AuthService.AuthorizeAsync(_currentUser, FSHPermissions.Roles.View)).Succeeded;

        await GetUsersAsync();
        _loaded = true;
    }

    private async Task GetUsersAsync()
    {
        var response = await _usersClient.GetAllAsync();
        if (response.Succeeded)
        {
            _userList = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private bool Search(UserDetailsDto user)
    {
        if (string.IsNullOrWhiteSpace(_searchString)) return true;
        if (user.FirstName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.LastName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.PhoneNumber?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.UserName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }

    private async Task InvokeModal()
    {
        var parameters = new DialogParameters();
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<RegisterUserModal>(_localizer["Register"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await GetUsersAsync();
        }
    }

    private void ViewProfile(Guid userId)
    {
        _navigationManager.NavigateTo($"/user-profile/{userId}");
    }

    private void ManageRoles(Guid userId, string email)
    {
        _navigationManager.NavigateTo($"/identity/user-roles/{userId}");
    }
}
