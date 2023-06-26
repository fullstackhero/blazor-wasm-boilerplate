using FL_CRMS_ERP_WASM.Client.Components.Dialogs;
using FL_CRMS_ERP_WASM.Client.Infrastructure.ApiClient;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Security.Cryptography;

namespace FL_CRMS_ERP_WASM.Client.Pages.LeadData.Account;

public partial class EditAccount
{
    [Parameter] public Guid _accountId { get; set; }
    bool _loaded;
    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        _loaded = true;
    }
    async Task LoadDataAsync()
    {
        await GetAllUserData();
        await GetAllAccount();
        await GetAccountById();
    }

    async Task GetAccountById()
    {
        try
        {
            var response = await _accountDetailClient.GetAsync(_accountId);
            if(response.Id != Guid.Empty)
            {
                _updateAccountRequest.AccountName = response.AccountName;
                _updateAccountRequest.AccountNumber = response.AccountNumber;
                _updateAccountRequest.AccountImage = response.AccountImage;
                _updateAccountRequest.UserId = response.UserId;
                _updateAccountRequest.AccountSite = response.AccountSite;
                _updateAccountRequest.AccountType = response.AccountType;
                _updateAccountRequest.BillingCity = response.BillingCity;
                _updateAccountRequest.BillingState = response.BillingState;
                _updateAccountRequest.BillingCode = response.BillingCode;
                _updateAccountRequest.BillingCountry = response.BillingCountry;
                _updateAccountRequest.BillingStreet = response.BillingStreet;
                _updateAccountRequest.ShippingCity = response.ShippingCity;
                _updateAccountRequest.ShippingStreet = response.ShippingStreet;
                _updateAccountRequest.ShippingCode = response.ShippingCode;
                _updateAccountRequest.ShippingCountry = response.ShippingCountry;
                _updateAccountRequest.ShippingState = response.ShippingState;
                _updateAccountRequest.Description = response.Description;
                _updateAccountRequest.Website = response.Website;
                _updateAccountRequest.ParentAccountId = response.ParentAccountId;
                _updateAccountRequest.Phone = response.Phone;
                _updateAccountRequest.Employees = response.Employees;
                _updateAccountRequest.Fax = response.Fax;
                _updateAccountRequest.Id = response.Id;
            }
        }
        catch(Exception ex)
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

    private async Task<IEnumerable<Guid>> SearchUser(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
            return _userDetailsDtos.Select(x => x.Id);

        return _userDetailsDtos.Where(x => x.FirstName.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Id);
    }

    [Inject] IAccountDetailsClient _accountDetailClient { get; set; }
    List<AccountDto> _accountDetailDto = new();
    async Task GetAllAccount()
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

    private async Task<IEnumerable<Guid?>> SearchAccount(string value)
    {
        // In real life use an asynchronous function for fetching data from an api.
        await Task.Delay(5);

        // if text is null or empty, show complete list
        if (string.IsNullOrEmpty(value))
            return _accountDetailDto.Select(x => (Guid?)x.Id);

        return _accountDetailDto.Where(x => x.AccountName.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => (Guid?)x.Id);
    }

    void CopyAddressBilling()
    {
        _updateAccountRequest.ShippingCity = _updateAccountRequest.BillingCity;
        _updateAccountRequest.ShippingStreet = _updateAccountRequest.BillingStreet;
        _updateAccountRequest.ShippingCode = _updateAccountRequest.BillingCode;
        _updateAccountRequest.ShippingCountry = _updateAccountRequest.BillingCountry;
        _updateAccountRequest.ShippingState = _updateAccountRequest.BillingState;
    }
    void CopyAddressShipping()
    {
        _updateAccountRequest.BillingCity = _updateAccountRequest.ShippingCity;
        _updateAccountRequest.BillingStreet = _updateAccountRequest.ShippingStreet;
        _updateAccountRequest.BillingCode = _updateAccountRequest.ShippingCode;
        _updateAccountRequest.BillingCountry = _updateAccountRequest.ShippingCountry;
        _updateAccountRequest.BillingState = _updateAccountRequest.ShippingState;
    }

    [Inject] IAccountDetailsClient _accountDetailsClient { get; set; }
    UpdateAccountRequest _updateAccountRequest = new();

    async Task UpdateAsync()
    {
        try
        {

            var response = await _accountDetailsClient.UpdateAsync(_accountId, _updateAccountRequest); ;
            if (response != Guid.Empty)
            {
                Snackbar.Add("Account Updated Successfully", Severity.Info);

            }

        }
        catch (ApiException<HttpValidationProblemDetails> ex)
        {
            if (ex.Result.Errors is not null)
            {
                Snackbar.Add(ex.Result.Title, Severity.Error);
            }
            else
            {
                Snackbar.Add("Something went wrong!", Severity.Error);
            }
        }
        catch (ApiException<ErrorResult> ex)
        {
            Snackbar.Add(ex.Result.Exception, Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    async Task Cancel()
    {
        Navigation.NavigateTo("/leaddata/accountinformation");
    }

    async Task DeleteAsync()
    {
        string deleteContent = "You're sure you want to Delete?";
        var parameters = new DialogParameters
                {
                { nameof(DeleteConfirmation.ContentText), string.Format(deleteContent, "Remove") }
                };
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<DeleteConfirmation>("Remove", parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            try
            {

                var response1 = await _accountDetailClient.DeleteAsync(_accountId);
                if (response1 != Guid.Empty)
                {
                    Snackbar.Add("Account Deleted Successfully", Severity.Info);
                    Navigation.NavigateTo("/leaddata/accountinformation");
                }

            }
            catch (ApiException<HttpValidationProblemDetails> ex)
            {
                if (ex.Result.Errors is not null)
                {
                    Snackbar.Add(ex.Result.Title, Severity.Error);
                }
                else
                {
                    Snackbar.Add("Something went wrong!", Severity.Error);
                }
            }
            catch (ApiException<ErrorResult> ex)
            {
                Snackbar.Add(ex.Result.Exception, Severity.Error);
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }

    }
}
