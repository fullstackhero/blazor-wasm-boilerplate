using FL_CRMS_ERP_WASM.Client.Components.EntityTable;
using FL_CRMS_ERP_WASM.Client.Infrastructure.ApiClient;
using FL_CRMS_ERP_WASM.Client.Infrastructure.Auth;
using FL.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FL_CRMS_ERP_WASM.Client.Pages.Identity.Users;

public partial class Users
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject]
    protected IUsersClient UsersClient { get; set; } = default!;

    protected EntityClientTableContext<UserDetailsDto, Guid, CreateUserRequest> Context { get; set; } = default!;

    private bool _canExportUsers;
    private bool _canViewRoles;

    // Fields for editform
    protected string Password { get; set; } = string.Empty;
    protected string ConfirmPassword { get; set; } = string.Empty;

    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

    protected override async Task OnInitializedAsync()
    {
        var user = (await AuthState).User;
        _canExportUsers = await AuthService.HasPermissionAsync(user, FLAction.Export, FLResource.Users);
        _canViewRoles = await AuthService.HasPermissionAsync(user, FLAction.View, FLResource.UserRoles);

        Context = new(
            entityName: L["User"],
            entityNamePlural: L["Users"],
            entityResource: FLResource.Users,
            searchAction: FLAction.View,
            updateAction: string.Empty,
            deleteAction: string.Empty,
            fields: new()
            {
                new(user => user.FirstName, L["First Name"]),
                new(user => user.LastName, L["Last Name"]),
                new(user => user.UserName, L["UserName"]),
                new(user => user.Email, L["Email"]),
                new(user => user.PhoneNumber, L["PhoneNumber"]),
                new(user => user.EmailConfirmed, L["Email Confirmation"], Type: typeof(bool)),
                new(user => user.IsActive, L["Active"], Type: typeof(bool))
            },
            idFunc: user => user.Id,
            loadDataFunc: async () => (await UsersClient.GetListAsync()).ToList(),
            searchFunc: (searchString, user) =>
                string.IsNullOrWhiteSpace(searchString)
                    || user.FirstName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.LastName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.Email?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.PhoneNumber?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
                    || user.UserName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true,
            createFunc: user => UsersClient.CreateAsync(user),
            hasExtraActionsFunc: () => true,
            exportAction: string.Empty);

            await GetAllUsers();
    }
    [Inject] IUsersClient _usersClient { get; set; }
    List<UserDetailsDto> _userDetailsDtoList = new();

    async Task GetAllUsers()
    {
        try
        {
            _userDetailsDtoList = (await _usersClient.GetListAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
    private async Task<IEnumerable<string>> SearchUsers(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
            return _userDetailsDtoList.Select(x => x.Id.ToString());

        return _userDetailsDtoList.Where(x => x.UserName.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Id.ToString());
    }

    private void ViewProfile(in Guid userId) =>
        Navigation.NavigateTo($"/users/{userId}/profile");

    private void ManageRoles(in Guid userId) =>
        Navigation.NavigateTo($"/users/{userId}/roles");

    private void TogglePasswordVisibility()
    {
        if (_passwordVisibility)
        {
            _passwordVisibility = false;
            _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
            _passwordInput = InputType.Password;
        }
        else
        {
            _passwordVisibility = true;
            _passwordInputIcon = Icons.Material.Filled.Visibility;
            _passwordInput = InputType.Text;
        }

        Context.AddEditModal.ForceRender();
    }
}