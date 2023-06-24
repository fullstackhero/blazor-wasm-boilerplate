using System.Security.Claims;
using FL_CRMS_ERP_WASM.Client.Components.Dialogs;
using FL_CRMS_ERP_WASM.Client.Infrastructure.ApiClient;
using FL_CRMS_ERP_WASM.Client.Pages.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FL_CRMS_ERP_WASM.Client.Pages.LeadData.Account;

public partial class AccountInformationById
{
    [Parameter] public Guid _accountId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }
   string _allDay;
    async Task LoadDataAsync()
    {
        await GetCurrentUserId();
        await GetAllAccountDetail();
        await GetAllUserData();
        await GetbyIdAccountDetail();
        await GetByIdUserData(_accountDetailDto.UserId);
        await GetAllTask();
        await GetAllUnCompletedTask();
        await GetAllCompletedTask();
        await GetAllMeeting();
        await GetAllCompletedMeeting();
        await GetAllUnCompletedMeeting();
        await GetAllCalls();
        await GetAllCompletedCalls();
        await GetAllUnCompletedCalls();

    }
     [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    string? _userId;
    async Task GetCurrentUserId()
    {
        var user = (await AuthState).User;
        if (user.Identity?.IsAuthenticated == true)
        {
            _userId = user.GetUserId();
        }
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
            _createNoteRequest.NoteOwnerId = Guid.Parse(_userId);
            _createNoteRequest.ParentId = _accountId;
            _createNoteRequest.RelatedTo = "Account";
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
   
    [Inject] ITaskDetailsClient _taskDetailsClient { get; set; }

    List<TaskDto> _taskDtos = new();
    async Task GetAllTask()
    {
        try
        {
            _taskDtos = (await _taskDetailsClient.GetListAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
     List<TaskDto> _unCompletedTaskDtos = new();
    async Task GetAllUnCompletedTask()
    {
        try
        {
            _unCompletedTaskDtos = (await _taskDetailsClient.GetListAsync()).Where(x => x.WhoId == _accountId && x.Status != "Completed").ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    List<TaskDto> _completedTaskDtos = new();
    async Task GetAllCompletedTask()
    {
        try
        {
            _completedTaskDtos = (await _taskDetailsClient.GetListAsync()).Where(x => x.WhoId == _accountId && x.Status == "Completed").ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
    async Task AddTask()
    {
        var parameters = new DialogParameters();
        if (_accountId != Guid.Empty)
        {
            parameters.Add(nameof(AddTaskDetailsDialog._createTaskRequest), new CreateTaskRequest
                {
                    WhoId = _accountId,
                    RelatedTo = "Account",
                    TaskOwnerId = Guid.Parse(_userId),
                    Status = "Not Started"
                });
        }
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<AddTaskDetailsDialog>("Create", parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await GetAllCompletedTask();
            await GetAllUnCompletedTask();
            await GetAllTask();
            //await TimeLine();
            //await Reset();
        }
    }

    async Task DeleteTask(Guid id)
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

                var response = await _taskDetailsClient.DeleteAsync(id);
                if (response != Guid.Empty)
                {
                    Snackbar.Add("Task Deleted Successfully", Severity.Info);
                    await GetAllCompletedTask();
                    await GetAllUnCompletedTask();
                    await GetAllTask();
                    //await TimeLine();
                    //Navigation.NavigateTo("/leaddata/leadinformation");
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

    async Task EditTask(Guid id)
    {
        var parameters = new DialogParameters();
        if (id != Guid.Empty)
        {
            TaskDto taskDto = new();
            taskDto = _taskDtos.FirstOrDefault(c => c.Id == id);
            if (taskDto != null)
            {
                parameters.Add(nameof(AddTaskDetailsDialog._updateTaskRequest), new UpdateTaskRequest
                    {
                        Id = taskDto.Id,
                        Status = taskDto.Status,
                        Description = taskDto.Description,
                        DueDate = taskDto.DueDate,
                        Remainder = taskDto.Remainder,
                        TaskOwnerId = taskDto.TaskOwnerId,
                        Priority = taskDto.Priority,
                        RelatedTo = taskDto.RelatedTo,
                        Subject = taskDto.Subject,
                        WhatId = taskDto.WhatId,
                        WhoId = taskDto.WhoId
                    });
            }
        }
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<AddTaskDetailsDialog>(id == Guid.Empty ? "Create" : "Edit", parameters, options);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            await GetAllCompletedTask();
            await GetAllUnCompletedTask();
            await GetAllTask();
           // await TimeLine();
            //await OpenActivitiesCount();
            //await ClosedActivitiesCount();
            StateHasChanged();
        }
    }

    [Inject] IMeetingDetailsClient _meetingDetailsClient { get; set; }

    List<MeetingDto> _meetingDtos = new();
    async Task GetAllMeeting()
    {
        try
        {
            _meetingDtos = (await _meetingDetailsClient.GetListAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    List<MeetingDto> _completedMeetingDto = new();

    async Task GetAllCompletedMeeting()
    {
        try
        {
            _completedMeetingDto = (await _meetingDetailsClient.GetListAsync()).Where(x => x.CheckedInStatus == true && x.Participants.Contains(_accountId.ToString())).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    List<MeetingDto> _unCompletedMeetingDto = new();
    async Task GetAllUnCompletedMeeting()
    {
        try
        {
            _unCompletedMeetingDto = (await _meetingDetailsClient.GetListAsync()).Where(x => x.CheckedInStatus == false && x.Participants.Contains(_accountId.ToString())).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    async Task AddMeeting()
    {
        var parameters = new DialogParameters();
        if (_accountId != Guid.Empty)
        {
            parameters.Add(nameof(AddMeetingDetailsDialog._createMeetingRequest), new CreateMeetingRequest
                {
                    WhoId = _accountId,
                    RelatedTo = "Account",
                    MeetingOwnerId = Guid.Parse(_userId)
                });
        }
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<AddMeetingDetailsDialog>("Create", parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await GetAllCompletedMeeting();
            await GetAllUnCompletedMeeting();
            await GetAllMeeting();
           // await UpcomingActionTask();
           // await UpcomingAuctionCall();
          //  await UpcomingAuctionMetting();
           // await TimeLine();
            //await Reset();
        }
    }

    async Task DeleteMeeting(Guid id)
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

                var response = await _meetingDetailsClient.DeleteAsync(id);
                if (response != Guid.Empty)
                {
                    Snackbar.Add("Meeting Deleted Successfully", Severity.Info);
                    await GetAllCompletedMeeting();
                    await GetAllUnCompletedMeeting();
                    await GetAllMeeting();
                   // await UpcomingActionTask();
                   // await UpcomingAuctionCall();
                   // await UpcomingAuctionMetting();
                   // await TimeLine();
                    //Navigation.NavigateTo("/leaddata/leadinformation");
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

    async Task EditMeeting(Guid id)
    {
        var parameters = new DialogParameters();
        if (id != Guid.Empty)
        {
            MeetingDto meetingDto = new();
            meetingDto = _meetingDtos.FirstOrDefault(c => c.Id == id);
            if (meetingDto != null)
            {
                parameters.Add(nameof(AddMeetingDetailsDialog._updateMeetingRequest), new UpdateMeetingRequest
                    {
                        Id = meetingDto.Id,
                        MeetingTitle = meetingDto.MeetingTitle,
                        Location = meetingDto.Location,
                        Allday = meetingDto.Allday,
                        FromDate = meetingDto.FromDate,
                        ToDate = meetingDto.ToDate,
                        Host = meetingDto.Host,
                        MeetingOwnerId = meetingDto.MeetingOwnerId,
                        WhoId = meetingDto.WhoId,
                        RelatedTo = meetingDto.RelatedTo,
                        WhatId = meetingDto.WhatId,
                        Repeat = meetingDto.Repeat,
                        Description = meetingDto.Description,
                        Participants = meetingDto.Participants,
                        RemindMe = meetingDto.RemindMe,
                        CheckedInStatus = meetingDto.CheckedInStatus
                    });
            }
        }
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<AddMeetingDetailsDialog>(id == Guid.Empty ? "Create" : "Edit", parameters, options);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            await GetAllCompletedMeeting();
            await GetAllUnCompletedMeeting();
            await GetAllMeeting();
          //  await UpcomingActionTask();
           // await UpcomingAuctionCall();
           // await UpcomingAuctionMetting();
           // await TimeLine();
            //await OpenActivitiesCount();
            //await ClosedActivitiesCount();
        }
    }
    

    [Inject] ICallDetailsClient _callDetailsClient { get; set; }
    List<CallDto> _callDtos = new();
    async Task GetAllCalls()
    {
        try
        {
            _callDtos = (await _callDetailsClient.GetListAsync()).ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    List<CallDto> _completedCallDtos = new();

    async Task GetAllCompletedCalls()
    {
        try
        {
            _completedCallDtos = (await _callDetailsClient.GetListAsync()).Where(x => x.WhoId == _accountId && x.OutgoingCallStatus == "Completed").ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    List<CallDto> _unCompletedCallDtos = new();
    async Task GetAllUnCompletedCalls()
    {
        try
        {
            _unCompletedCallDtos = (await _callDetailsClient.GetListAsync()).Where(x => x.WhoId == _accountId && x.OutgoingCallStatus != "Completed").ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    async Task AddCall()
    {
        var parameters = new DialogParameters();
        if (_accountId != Guid.Empty)
        {


            parameters.Add(nameof(AddCallDetailsDialog._createCallRequest), new CreateCallRequest
                {
                    WhoId = _accountId,
                    RelatedTo = "Account",
                    CallOwnerId = Guid.Parse(_userId),
                    OutgoingCallStatus = "Scheduled",
                    CallType = "Outbound",
                    CallPurpose = "Administrative"
                });

        }

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<AddCallDetailsDialog>("Create", parameters, options);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            await GetAllCompletedCalls();
            await GetAllUnCompletedCalls();
            await GetAllCalls();
           // await UpcomingActionTask();
           // await UpcomingAuctionCall();
            //await UpcomingAuctionMetting();
           // await TimeLine();
            //await ResetCalls();
        }
    }

    async Task DeleteCall(Guid id)
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

                var response = await _callDetailsClient.DeleteAsync(id);
                if (response != Guid.Empty)
                {
                    Snackbar.Add("Call Deleted Successfully", Severity.Info);
                    await GetAllCompletedCalls();
                    await GetAllUnCompletedCalls();
                    await GetAllCalls();
                   // await UpcomingActionTask();
                   // await UpcomingAuctionCall();
                    //await UpcomingAuctionMetting();
                   // await TimeLine();
                    //Navigation.NavigateTo("/leaddata/leadinformation");
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

    async Task EditCall(Guid id)
    {
        var parameters = new DialogParameters();
        if (id != Guid.Empty)
        {
            CallDto callDto = new();
            callDto = _callDtos.FirstOrDefault(c => c.Id == id);
            if (callDto != null)
            {
                parameters.Add(nameof(AddCallDetailsDialog._updateCallRequest), new UpdateCallRequest
                    {
                        Id = callDto.Id,
                        CallAgenda = callDto.CallAgenda,
                        Description = callDto.Description,
                        CallStartTime = callDto.CallStartTime,
                        CallPurpose = callDto.CallPurpose,
                        CallOwnerId = callDto.CallOwnerId,
                        CallType = callDto.CallType,
                        OutgoingCallStatus = callDto.OutgoingCallStatus,
                        Subject = callDto.Subject,
                        WhatId = callDto.WhatId,
                        WhoId = callDto.WhoId,
                        RemainderTime = callDto.RemainderTime,
                        RelatedTo = callDto.RelatedTo
                    });
            }
        }
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
        var dialog = DialogService.Show<AddCallDetailsDialog>(id == Guid.Empty ? "Create" : "Edit", parameters, options);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await GetAllCompletedCalls();
            await GetAllUnCompletedCalls();
            await GetAllCalls();
            //await UpcomingActionTask();
            //await UpcomingAuctionCall();
            //await UpcomingAuctionMetting();
           // await TimeLine();
            //await OpenActivitiesCount();
            //await ClosedActivitiesCount();
            //StateHasChanged();
        }
    }

    //......................
}
