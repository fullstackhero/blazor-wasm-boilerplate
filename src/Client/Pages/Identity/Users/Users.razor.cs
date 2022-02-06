using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Auth;
using FSH.WebApi.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity.Users;

public partial class Users
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject]
    protected IUsersClient UsersClient { get; set; } = default!;
    [Inject]
    protected IProfileClient ProfileClient { get; set; } = default!;

    protected EntityClientTableContext<UserDetailsDto, Guid, CreateProfileRequest> Context { get; set; } = default!;

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
        _canExportUsers = await AuthService.HasPermissionAsync(user, FSHAction.Export, FSHResource.Users);
        _canViewRoles = await AuthService.HasPermissionAsync(user, FSHAction.View, FSHResource.UserRoles);

        Context = new(
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
            searchFunc: Search,
            createFunc: async user => await ProfileClient.CreateAsync(user),
            entityName: L["User"],
            entityNamePlural: L["Users"],
            searchPermission: FSHPermission.GetName(FSHAction.Search, FSHResource.Users),
            createPermission: FSHPermission.GetName(FSHAction.Create, FSHResource.Users),
            hasExtraActionsFunc: () => true);
    }

    private bool Search(string? searchString, UserDetailsDto user) =>
        string.IsNullOrWhiteSpace(searchString)
            || user.FirstName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
            || user.LastName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
            || user.Email?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
            || user.PhoneNumber?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
            || user.UserName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true;

    private void ViewProfile(Guid userId) =>
        Navigation.NavigateTo($"/users/{userId}/profile");

    private void ManageRoles(Guid userId) =>
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

        Context.AddEditModal?.ForceRender();
    }
}