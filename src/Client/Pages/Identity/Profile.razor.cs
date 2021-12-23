using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Authentication;
using FSH.BlazorWebAssembly.Client.Infrastructure.Authentication.Jwt;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity;

public partial class Profile
{
    private char _firstLetterOfName;
    private readonly UpdateProfileRequest _profileModel = new();
    [Inject]
    private IIdentityClient _identityClient { get; set; } = default!;
    [Inject]
    private IAuthenticationService _authService { get; set; } = default!;
    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    public string UserId { get; set; }

    private async Task UpdateProfileAsync()
    {
        var response = await _identityClient.UpdateProfileAsync(_profileModel);
        if (response.Succeeded)
        {
            await _authService.LogoutAsync();
            _snackBar.Add(_localizer["Your Profile has been updated. Please Login to Continue."], Severity.Success);
            _navigationManager.NavigateTo("/");
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var state = await AuthState;
        var user = state.User;
        _profileModel.Email = user.GetEmail();
        _profileModel.FirstName = user.GetFirstName();
        _profileModel.LastName = user.GetSurname();
        _profileModel.PhoneNumber = user.GetPhoneNumber();
        ImageDataUrl = user.GetImageUrl().Replace("{server_url}/", _configurations[ConfigNames.ApiBaseUrl]);
        UserId = user.GetUserId();

        if (_profileModel.FirstName?.Length > 0)
        {
            _firstLetterOfName = _profileModel.FirstName[0];
        }
    }

    private IBrowserFile _file;

    [Parameter]
    public string ImageDataUrl { get; set; }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        _file = e.File;
        if (_file != null)
        {
            var supportedFormats = new List<string> { ".jpeg", ".jpg", ".png" };
            string? extension = Path.GetExtension(_file.Name);
            if(!supportedFormats.Contains(extension.ToLower()))
            {
                _snackBar.Add("File Format Not Supported.", Severity.Error);
                return;
            }

            string? fileName = $"{UserId}-{Guid.NewGuid().ToString("N")}";
            fileName = fileName.Substring(0, Math.Min(fileName.Length, 90));
            string? format = "image/png";
            var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
            byte[]? buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            string? base64String = $"data:image/png;base64,{Convert.ToBase64String(buffer)}";
            _profileModel.Image = new FileUploadRequest() { Name = fileName, Data = base64String, Extension = extension };
            var response = await _identityClient.UpdateProfileAsync(_profileModel);
            if (response.Succeeded)
            {
                await _authService.LogoutAsync();
                _snackBar.Add(_localizer["Your Profile has been updated. Please Login to Continue."], Severity.Success);
                _navigationManager.NavigateTo("/");
            }
            else
            {
                foreach (string? message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }

    private async Task DeleteAsync()
    {
        var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), $"{string.Format(_localizer["Do you want to delete the profile picture of {0}"], _profileModel.Email)}?"}
            };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            //var request = new UpdateProfilePictureRequest { Data = null, FileName = string.Empty, UploadType = Application.Enums.UploadType.ProfilePicture };
            //var data = await _accountManager.UpdateProfilePictureAsync(request, UserId);
            //if (data.Succeeded)
            //{
            //    await _localStorage.RemoveItemAsync(StorageConstants.Local.UserImageURL);
            //    ImageDataUrl = string.Empty;
            //    _snackBar.Add(_localizer["Profile picture deleted."], Severity.Success);
            //    _navigationManager.NavigateTo("/account", true);
            //}
            //else
            //{
            //    foreach (var error in data.Messages)
            //    {
            //        _snackBar.Add(error, Severity.Error);
            //    }
            //}
        }
    }
}