using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using FSH.BlazorWebAssembly.Client.Infrastructure.Authentication;
using FSH.BlazorWebAssembly.Client.Infrastructure.Common;
using FSH.BlazorWebAssembly.Shared.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Identity;

public partial class Profile
{
    private readonly UpdateProfileRequest _profileModel = new();
    private char _firstLetterOfName;

    [Inject]
    private IIdentityClient _identityClient { get; set; } = default!;

    [Inject]
    private IAuthenticationService _authService { get; set; } = default!;

    [CascadingParameter]
    public Task<AuthenticationState> AuthState { get; set; } = default!;

    public string? UserId { get; set; }

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
            if (response.Messages != null)
            {
                foreach (string? message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
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
        if (user != null)
        {
            _profileModel.Email = user.GetEmail() ?? string.Empty;
            _profileModel.FirstName = user.GetFirstName() ?? string.Empty;
            _profileModel.LastName = user.GetSurname() ?? string.Empty;
            _profileModel.PhoneNumber = user.GetPhoneNumber();
            ImageDataUrl = user?.GetImageUrl()?.Replace("{server_url}/", _configurations[ConfigNames.ApiBaseUrl]);
            UserId = user?.GetUserId();
        }

        if (_profileModel.FirstName?.Length > 0)
        {
            _firstLetterOfName = _profileModel.FirstName[0];
        }
    }

    private IBrowserFile? _file;

    [Parameter]
    public string? ImageDataUrl { get; set; }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        _file = e.File;
        if (_file != null)
        {
            var supportedFormats = new List<string> { ".jpeg", ".jpg", ".png" };
            string? extension = Path.GetExtension(_file.Name);
            if (!supportedFormats.Contains(extension.ToLower()))
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
                if (response.Messages != null)
                {
                    foreach (string? message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }
    }
}