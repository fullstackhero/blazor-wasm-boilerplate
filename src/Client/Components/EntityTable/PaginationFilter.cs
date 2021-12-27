namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

public class PaginationFilter
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public string? Keyword { get; set; }
    public string[]? OrderBy { get; set; }
}
