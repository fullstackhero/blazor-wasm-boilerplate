using Microsoft.AspNetCore.Components;

namespace FL_CRMS_ERP_WASM.Client.Pages.LeadData.Account;

public partial class EditAccount
{
    [Parameter] public Guid _accountId { get; set; }
    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }
    async Task LoadDataAsync()
    {

    }
}
