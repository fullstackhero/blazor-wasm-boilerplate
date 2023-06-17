using FL_CRMS_ERP_WASM.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FL_CRMS_ERP_WASM.Client.Pages.LeadData.Account;

public partial class AccountInformation
{
    protected async override Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }
    async Task LoadDataAsync()
    {
        await GetAllUserData();
        await GetAllAccountDetail();
    }

    [Inject] IAccountDetailsClient _accountDetailClient { get; set; }
    List<AccountDto> _accountDetailDto = new();
    async Task GetAllAccountDetail()
    {
        try
        {
            _accountDetailDto = (await _accountDetailClient.GetListAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
    [Inject] IUsersClient _usersClient { get; set; }
    List<UserDetailsDto> _userDetailsDtos = new();

    async Task GetAllUserData()
    {
        try
        {
            _userDetailsDtos = (await _usersClient.GetListAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private MudTable<AccountDto> mudTable;
    string SelectedRowClassFunc(AccountDto element, int rowNumber)
    {

        if (mudTable.SelectedItem != null && mudTable.SelectedItem.Equals(element))
        {
            Guid id = element.Id;
            clickedEvents.Add($"Selected Row: {rowNumber}");
            NavInvoiceDetails(id);
            return "selected";
        }
        else
        {
            return string.Empty;
        }
    }

    private List<string> clickedEvents = new();
    private void RowClickEvent(TableRowClickEventArgs<AccountDto> tableRowClickEventArgs)
    {
        clickedEvents.Add("Row has been clicked");
    }

    async Task NavInvoiceDetails(Guid id)
    {
        Navigation.NavigateTo($"/leaddata/accountinformationbyid/{id}");
    }

    //.................................
}
