using FSH.BlazorWebAssembly.Client.Infrastructure.Services.Personal.AuditLogs;
using FSH.BlazorWebAssembly.Shared.Response.AuditLogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Security.Claims;

namespace FSH.BlazorWebAssembly.Client.Pages.Personal;
public partial class AuditLogs
{
    [Inject] private IAuditLogsService AuditService { get; set; }

    public List<RelatedAuditTrail> Trails = new();

    private RelatedAuditTrail _trail = new();
    private string _searchString = "";
    private bool _dense = true;
    private bool _striped = true;
    private bool _bordered = false;
    private bool _searchInOldValues = false;
    private bool _searchInNewValues = false;
    private MudDateRangePicker _dateRangePicker;
    private DateRange _dateRange;

    private ClaimsPrincipal _currentUser;
    private bool _canExportAuditTrails;
    private bool _canSearchAuditTrails;
    private bool _loaded;

    private bool Search(AuditResponse response)
    {
        var result = false;

        // check Search String
        if (string.IsNullOrWhiteSpace(_searchString)) result = true;
        if (!result)
        {
            if (response.TableName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                result = true;
            }
            if (_searchInOldValues &&
                response.OldValues?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                result = true;
            }
            if (_searchInNewValues &&
                response.NewValues?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                result = true;
            }
        }

        // check Date Range
        if (_dateRange?.Start == null && _dateRange?.End == null) return result;
        if (_dateRange?.Start != null && response.DateTime < _dateRange.Start)
        {
            result = false;
        }
        if (_dateRange?.End != null && response.DateTime > _dateRange.End + new TimeSpan(0, 11, 59, 59, 999))
        {
            result = false;
        }

        return result;
    }

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await _authService.CurrentUser();
        _canExportAuditTrails = true; //(await _authorizationService.AuthorizeAsync(_currentUser, Permissions.AuditTrails.Export)).Succeeded;
        _canSearchAuditTrails = true; //(await _authorizationService.AuthorizeAsync(_currentUser, Permissions.AuditTrails.Search)).Succeeded;

        await GetDataAsync();
        _loaded = true;
    }

    private async Task GetDataAsync()
    {
        var response = await AuditService.GetCurrentUserAuditLogsAsync();
        if (response.Succeeded)
        {
            Trails = response.Data
                .Select(x => new RelatedAuditTrail
                {
                    AffectedColumns = x.AffectedColumns,
                    DateTime = x.DateTime,
                    Id = x.Id,
                    NewValues = x.NewValues,
                    OldValues = x.OldValues,
                    PrimaryKey = x.PrimaryKey,
                    TableName = x.TableName,
                    Type = x.Type,
                    UserId = x.UserId,
                    LocalTime = DateTime.SpecifyKind(x.DateTime, DateTimeKind.Utc).ToLocalTime()
                }).ToList();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private void ShowBtnPress(Guid id)
    {
        _trail = Trails.First(f => f.Id == id);
        foreach (var trial in Trails.Where(a => a.Id != id))
        {
            trial.ShowDetails = false;
        }
        _trail.ShowDetails = !_trail.ShowDetails;
    }
    public class RelatedAuditTrail : AuditResponse
    {
        public bool ShowDetails { get; set; } = false;
        public DateTime LocalTime { get; set; }
    }
}