using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity.Users;

public partial class UserRoles
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Parameter]
    public string? Id { get; set; }
    [Parameter]
    public string? Title { get; set; }
    [Parameter]
    public string? Description { get; set; }
    [Inject]
    protected IUsersClient UsersClient { get; set; } = default!;
    public List<UserRoleDto> UserRolesList { get; set; } = new();

    private UserRoleDto _userRole = new();
    private string _searchString = string.Empty;
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;

    private bool _canEditUsers;
    private bool _canSearchRoles;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canEditUsers = (await AuthService.AuthorizeAsync(state.User, FSHPermissions.Users.Edit)).Succeeded;
        _canSearchRoles = (await AuthService.AuthorizeAsync(state.User, FSHPermissions.Roles.View)).Succeeded;

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => UsersClient.GetByIdAsync(Id), Snackbar)
            is UserDetailsDto user)
        {
            Title = $"{user.FirstName} {user.LastName}";
            Description = string.Format(_localizer["Manage {0} {1}'s Roles"], user.FirstName, user.LastName);

            if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => UsersClient.GetRolesAsync(user.Id.ToString()), Snackbar)
                is ICollection<UserRoleDto> response)
            {
                UserRolesList = response.ToList();
            }
        }

        _loaded = true;
    }

    private async Task SaveAsync()
    {
        var request = new UserRolesRequest()
        {
            UserRoles = UserRolesList
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => UsersClient.AssignRolesAsync(Id, request),
            Snackbar,
            new CustomValidation(),
            _localizer["Success"]) is not null)
        {
            Navigation.NavigateTo($"/users");
        }
    }

    private bool Search(UserRoleDto userRole)
    {
        if (string.IsNullOrWhiteSpace(_searchString)) return true;
        if (userRole.RoleName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }
}