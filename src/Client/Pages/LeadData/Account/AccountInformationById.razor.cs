using FL_CRMS_ERP_WASM.Client.Components.Dialogs;
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
        await GetAllUserData();
        await GetbyIdAccountDetail();
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
    async Task GetbyIdAccountDetail()
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

    List<AccountDto> _accountDetailList = new();
    async Task GetAllAccountDetail()
    {
        try
        {
            _accountDetailList = (await _accountDetailClient.GetListAsync()).ToList();
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

    List<UserDetailsDto> _userDetailsList = new();

    async Task GetAllUserData()
    {
        try
        {
            _userDetailsList = (await _usersClient.GetListAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
    
    public bool _expand;
    public string _expandText = "Show Details";

    public void ExpandChange()
    {
        if (_expand == false)
        {
            _expand = true;
            _expandText = "Hide Details";
        }
        else
        {
            _expand = false;
            _expandText = "Show Details";
        }
    }

    [Inject] INoteDetailsClient _noteDetailsClient { get; set; }
    List<NotesDto> _notesDtos = new();
    async Task RecentFirst()
    {
        try
        {
            _notesDtos = (await _noteDetailsClient.GetListAsync()).Where(x => x.ParentId == _accountId).ToList();
            _notesDtos.Reverse();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    async Task RecentLast()
    {
        try
        {
            _notesDtos = (await _noteDetailsClient.GetListAsync()).Where(x => x.ParentId == _accountId).ToList();
            _notesDtos.Reverse();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    async Task NotesCheckById(Guid id)
    {
        try
        {
            var response = await _noteDetailsClient.GetAsync(id);
            if (response.Id != Guid.Empty)
            {
                _updateNoteRequest.Id = response.Id;
                _updateNoteRequest.NoteContent = response.NoteContent;
                _updateNoteRequest.NoteTitle = response.NoteTitle;
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }


    }

    UpdateNoteRequest _updateNoteRequest = new();
    async Task UpdateNotes()
    {
        try
        {
            var response = await _noteDetailsClient.UpdateAsync(_updateNoteRequest.Id, _updateNoteRequest);
            if (response != Guid.Empty)
            {
                Snackbar.Add("Notes Updated Successfully", Severity.Info);
                await RecentLast();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    CreateNoteRequest _createNoteRequest = new();
    async Task SaveNotes()
    {
        try
        {
            var response1 = await _noteDetailsClient.CreateAsync(_createNoteRequest);
            if (response1 != Guid.Empty)
            {
                Snackbar.Add("Notes Added Successfully", Severity.Info);
                await RecentLast();
                //Navigation.NavigateTo("/view/stock");
                //await Reset();
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

    async Task DeleteNotes()
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

                var response1 = await _noteDetailsClient.DeleteAsync(_updateNoteRequest.Id);
                if (response1 != Guid.Empty)
                {
                    Snackbar.Add("Notes Deleted Successfully", Severity.Info);
                    _updateNoteRequest.Id = Guid.Empty;
                    await RecentLast();
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
    
    async Task CancelNotes()
    {
        _updateNoteRequest.Id = Guid.Empty;
        _updateNoteRequest.NoteContent = string.Empty;
    }

    //......................
}
