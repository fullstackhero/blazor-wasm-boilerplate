using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Authentication;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
using FSH.BlazorWebAssembly.Client.Shared;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity.Account;

public partial class Profile
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IAuthenticationService AuthService { get; set; } = default!;
    [Inject]
    protected IIdentityClient IdentityClient { get; set; } = default!;

    private readonly UpdateProfileRequest _profileModel = new();

    private string? _imageUrl;
    private string? _userId;
    private char _firstLetterOfName;

    private CustomValidation? _customValidation;

    protected override async Task OnInitializedAsync()
    {
        var state = await AuthState;
        var user = state.User;
        if (user is not null)
        {
            _userId = user.GetUserId();
            _profileModel.Email = user.GetEmail() ?? string.Empty;
            _profileModel.FirstName = user.GetFirstName() ?? string.Empty;
            _profileModel.LastName = user.GetSurname() ?? string.Empty;
            _profileModel.PhoneNumber = user.GetPhoneNumber();
            _imageUrl = user?.GetImageUrl()?.Replace("{server_url}/", Config[ConfigNames.ApiBaseUrl]);
            if (_userId != null) _profileModel.Id = _userId;
        }

        if (_profileModel.FirstName?.Length > 0)
        {
            _firstLetterOfName = _profileModel.FirstName[0];
        }
    }

    private async Task UpdateProfileAsync()
    {
        if (await ApiHelper.ExecuteCallGuardedAsync(
            () => IdentityClient.UpdateProfileAsync(_profileModel), Snackbar, _customValidation))
        {
            Snackbar.Add(L["Your Profile has been updated. Please Login again to Continue."], Severity.Success);
            await AuthService.ReLoginAsync(Navigation.Uri);
        }
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file is not null)
        {
            string? extension = Path.GetExtension(file.Name);
            if (!ApplicationConstants.SupportedImageFormats.Contains(extension.ToLower()))
            {
                Snackbar.Add("Image Format Not Supported.", Severity.Error);
                return;
            }

            string? fileName = $"{_userId}-{Guid.NewGuid():N}";
            fileName = fileName[..Math.Min(fileName.Length, 90)];
            var imageFile = await file.RequestImageFileAsync(ApplicationConstants.StandardImageFormat, ApplicationConstants.MaxImageWidth, ApplicationConstants.MaxImageHeight);
            byte[]? buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            string? base64String = $"data:{ApplicationConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}";
            _profileModel.Image = new FileUploadRequest() { Name = fileName, Data = base64String, Extension = extension };

            await UpdateProfileAsync();
        }
    }
}