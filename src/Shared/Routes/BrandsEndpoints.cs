namespace FSH.BlazorWebAssembly.Shared.Routes;

public static class BrandsEndpoints
{
    public static string ExportFiltered(string searchString)
    {
        return $"{Export}?searchString={searchString}";
    }

    public const string Export = "api/v1/brands/export";

    public const string Search = "api/v1/brands/search";
    public const string Delete = "api/v1/brands";
    public const string Save = "api/v1/brands/";
    public const string GetCount = "api/v1/brands/count";
    public const string Import = "api/v1/brands/import";
}