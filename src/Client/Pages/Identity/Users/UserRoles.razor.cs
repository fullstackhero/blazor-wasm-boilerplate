using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Auth;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity.Users;

public partial class UserRoles
{
    [Parameter]
    public string? Id { get; set; }
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;
    [Inject]
    protected IUsersClient UsersClient { get; set; } = default!;

    private List<UserRoleDto> _userRolesList = default!;

    private string _title = string.Empty;
    private string _description = string.Empty;

    private string _searchString = string.Empty;

    private bool _canEditUsers;
    private bool _canSearchRoles;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        _canEditUsers = await AuthService.HasPermissionAsync(state.User, FSHAction.Update, FSHResource.Users);
        _canSearchRoles = await AuthService.HasPermissionAsync(state.User, FSHAction.View, FSHResource.UserRoles);

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => UsersClient.GetByIdAsync(Id), Snackbar)
            is UserDetailsDto user)
        {
            _title = $"{user.FirstName} {user.LastName}";
            _description = string.Format(L["Manage {0} {1}'s Roles"], user.FirstName, user.LastName);

            if (await ApiHelper.ExecuteCallGuardedAsync(
                    () => UsersClient.GetRolesAsync(user.Id.ToString()), Snackbar)
                is ICollection<UserRoleDto> response)
            {
                _userRolesList = response.ToList();
            }
        }

        _loaded = true;
    }

    private async Task SaveAsync()
    {
        var request = new UserRolesRequest()
        {
            UserRoles = _userRolesList
        };

        if (await ApiHelper.ExecuteCallGuardedAsync(
                () => UsersClient.AssignRolesAsync(Id, request),
                Snackbar,
                successMessage: L["Updated User Roles."])
            is not null)
        {
            Navigation.NavigateTo("/users");
        }
    }

    private bool Search(UserRoleDto userRole) =>
        string.IsNullOrWhiteSpace(_searchString)
            || userRole.RoleName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) is true;
}