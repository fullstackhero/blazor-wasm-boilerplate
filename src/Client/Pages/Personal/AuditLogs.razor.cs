using FSH.BlazorWebAssembly.Client.Components.EntityTable;
using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;
using Mapster;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Pages.Personal;

public partial class AuditLogs
{
    [Inject]
    private IAuditLogsClient AuditLogsClient { get; set; } = default!;

    protected EntityClientTableContext<RelatedAuditTrail, Guid, object> Context { get; set; } = default!;

    private string? _searchString;
    private MudDateRangePicker _dateRangePicker = default!;
    private DateRange? _dateRange;
    private bool _searchInOldValues;
    private bool _searchInNewValues;
    private List<RelatedAuditTrail> _trails = new();

    // Configure Automapper
    static AuditLogs() =>
        TypeAdapterConfig<AuditDto, RelatedAuditTrail>.NewConfig().Map(
            dest => dest.LocalTime,
            src => DateTime.SpecifyKind(src.DateTime, DateTimeKind.Utc).ToLocalTime());

    protected override void OnInitialized()
    {
        Context = new(
            fields: new()
            {
                new(audit => audit.Id, L["Id"]),
                new(audit => audit.TableName, L["Table Name"]),
                new(audit => audit.DateTime, L["Date"], Template: DateFieldTemplate),
                new(audit => audit.Type, L["Type"])
            },
            loadDataFunc: async () => _trails = (await AuditLogsClient.GetMyLogsAsync()).Adapt<List<RelatedAuditTrail>>() ?? new List<RelatedAuditTrail>(),
            searchFunc: Search,
            searchPermission: true.ToString(),
            entityNamePlural: L["Trails"],
            hasExtraActionsFunc: () => true);
    }

    private bool Search(string? searchString, RelatedAuditTrail trail) =>
        (string.IsNullOrWhiteSpace(searchString) // check Search String
            || trail.TableName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true
            || (_searchInOldValues &&
                trail.OldValues?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true)
            || (_searchInNewValues &&
                trail.NewValues?.Contains(searchString, StringComparison.OrdinalIgnoreCase) == true))
        && ((_dateRange?.Start is null && _dateRange?.End is null) // check Date Range
            || (_dateRange?.Start is not null && _dateRange.End is null && trail.DateTime >= _dateRange.Start)
            || (_dateRange?.Start is null && _dateRange?.End is not null && trail.DateTime <= _dateRange.End + new TimeSpan(0, 11, 59, 59, 999))
            || (trail.DateTime >= _dateRange!.Start && trail.DateTime <= _dateRange.End + new TimeSpan(0, 11, 59, 59, 999)));

    private void ShowBtnPress(Guid id)
    {
        var trail = _trails.First(f => f.Id == id);
        trail.ShowDetails = !trail.ShowDetails;
        foreach (var otherTrail in _trails.Except(new[] { trail }))
        {
            otherTrail.ShowDetails = false;
        }
    }

    public class RelatedAuditTrail : AuditDto
    {
        public bool ShowDetails { get; set; }
        public DateTime LocalTime { get; set; }
    }
}