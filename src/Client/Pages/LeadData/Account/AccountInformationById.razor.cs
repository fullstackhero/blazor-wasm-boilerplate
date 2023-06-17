using FL_CRMS_ERP_WASM.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FL_CRMS_ERP_WASM.Client.Pages.LeadData.Account;

public partial class AccountInformationById
{
    [Parameter] public Guid _accountId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    async Task LoadDataAsync()
    {
        await GetAllAccountDetail();
        await GetByIdUserData(_accountDetailDto.UserId);

    }

    async Task Cancel()
    {
        Navigation.NavigateTo("/leaddata/accountinformation");
    }

    async Task EditAccount()
    {
        Navigation.NavigateTo($"/leaddata/editaccount/{_accountId}");
    }

    [Inject] IAccountDetailsClient _accountDetailClient { get; set; }
    AccountDto _accountDetailDto = new();
    async Task GetAllAccountDetail()
    {
        try
        {
            _accountDetailDto = (await _accountDetailClient.GetAsync(_accountId));
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    [Inject] IUsersClient _usersClient { get; set; }
    UserDetailsDto _userDetailsDtos = new();

    async Task GetByIdUserData( Guid _userId)
    {
        try
        {
            _userDetailsDtos = await _usersClient.GetByIdAsync(_userId.ToString());
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }


    //......................
}
