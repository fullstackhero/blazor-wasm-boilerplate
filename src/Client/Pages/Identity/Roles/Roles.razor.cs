using FSH.BlazorWebAssembly.Shared.Identity;
using FSH.BlazorWebAssembly.Shared.Requests.Identity;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity.Roles;

public partial class Roles
{
    private List<RoleDto> _roleList = new();
    private RoleDto _role = new();
    private string _searchString = string.Empty;
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;

    // private ClaimsPrincipal _currentUser;
    private bool _canCreateRoles;
    private bool _canEditRoles;
    private bool _canDeleteRoles;
    private bool _canSearchRoles;
    private bool _canViewRoleClaims;
    private bool _loading = true;

    public bool checkBox { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        // _currentUser = await _authService.CurrentUser();
        _canCreateRoles = true; // (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Create)).Succeeded;
        _canEditRoles = true; // (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Edit)).Succeeded;
        _canDeleteRoles = true; // (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Delete)).Succeeded;
        _canSearchRoles = true; // (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Search)).Succeeded;
        _canViewRoleClaims = true; // (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.View)).Succeeded;

        await GetRolesAsync();
        _loading = false;
    }

    private async Task GetRolesAsync()
    {
        _loading = true;
        var response = await _roleService.GetRolesAsync();
        if (response.Succeeded && response.Data is not null)
        {
            _roleList = response.Data.ToList();
        }
        else if (response.Messages is not null)
        {
            foreach (string message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        _loading = false;
    }

    private async Task Delete(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        string deleteContent = _localizer["Delete Content"];
        var parameters = new DialogParameters
            {
                { nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(deleteContent, id) }
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var response = await _roleService.DeleteAsync(id);
            if (response.Succeeded)
            {
                if (response.Messages?.Count > 0)
                {
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
            }
            else if (response.Messages is not null)
            {
                foreach (string message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            await Reset();
        }
    }

    private async Task InvokeModal(string? id = null)
    {
        var parameters = new DialogParameters();
        if (id is not null && _roleList.FirstOrDefault(c => c.Id == id) is RoleDto role)
        {
            _role = role;
            parameters.Add(nameof(RoleModal.RoleModel), new RoleRequest
            {
                Id = _role.Id,
                Name = _role.Name,
                Description = _role.Description
            });
        }

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<RoleModal>(id == null ? _localizer["Create"] : _localizer["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await Reset();
        }
    }

    private async Task Reset()
    {
        _role = new RoleDto();
        await GetRolesAsync();
    }

    private bool Search(RoleDto role)
    {
        if (string.IsNullOrWhiteSpace(_searchString)) return true;
        if (role.Name?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (role.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }

    private void ManagePermissions(string? roleId)
    {
        if (string.IsNullOrEmpty(roleId))
        {
            throw new ArgumentNullException(nameof(roleId));
        }

        _navigationManager.NavigateTo($"/identity/role-permissions/{roleId}");
    }
}