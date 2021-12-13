using System.Text.Json;
using System.Text.Json.Serialization;

namespace FSH.BlazorWebAssembly.Shared.Wrapper;

public static class ResultExtensions
{
    public static async Task<IResult<T>> ToResultAsync<T>(this HttpResponseMessage response)
    {
        string responseAsString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseAsString))
        {
            throw new InvalidOperationException($"API returned emtpy response. StatusCode: {response.StatusCode}");
        }

        return JsonSerializer.Deserialize<Result<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            })
            ?? throw new InvalidOperationException($"API returned null value. StatusCode: {response.StatusCode}");
    }

    public static async Task<IResult> ToResultAsync(this HttpResponseMessage response)
    {
        string responseAsString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseAsString))
        {
            throw new InvalidOperationException($"API returned emtpy response. StatusCode: {response.StatusCode}");
        }

        return JsonSerializer.Deserialize<Result>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            })
            ?? throw new InvalidOperationException($"API returned null value. StatusCode: {response.StatusCode}");
    }

    public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(this HttpResponseMessage response)
    {
        string responseAsString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseAsString))
        {
            throw new InvalidOperationException($"API returned emtpy response. StatusCode: {response.StatusCode}");
        }

        return JsonSerializer.Deserialize<PaginatedResult<T>>(responseAsString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })
            ?? throw new InvalidOperationException($"API returned null value. StatusCode: {response.StatusCode}");
    }
}