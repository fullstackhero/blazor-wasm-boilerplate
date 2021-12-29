using FSH.BlazorWebAssembly.Client.Infrastructure.ApiClient;

namespace FSH.BlazorWebAssembly.Client.Components.EntityTable;

public class Result<T> : Result
{
    public T? Data { get; set; }
}

public class ListResult<T> : Result<List<T>>
{
}