using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

public class ListResult<T> : Result
{
    public List<T>? Data { get; set; }
}
