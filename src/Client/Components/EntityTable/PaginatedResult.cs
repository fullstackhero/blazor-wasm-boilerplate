namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

public class PaginatedResult<T> : ListResult<T>
{
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
