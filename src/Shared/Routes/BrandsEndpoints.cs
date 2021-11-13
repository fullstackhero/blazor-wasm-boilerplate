namespace FSH.BlazorWebAssembly.Shared.Routes;
public static class BrandsEndpoints
{
    public static string ExportFiltered(string searchString)
    {
        return $"{Export}?searchString={searchString}";
    }

    public static string Export = "api/v1/brands/export";

    public static string Search = "api/v1/brands/search";
    public static string Delete = "api/v1/brands";
    public static string Save = "api/v1/brands/";
    public static string GetCount = "api/v1/brands/count";
    public static string Import = "api/v1/brands/import";
}