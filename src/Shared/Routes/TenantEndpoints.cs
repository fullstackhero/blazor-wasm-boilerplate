namespace FSH.BlazorWebAssembly.Shared.Routes;

public static class TenantEndpoints
{
    public const string GetAll = "api/tenants";
    public const string Create = "api/tenants";
    public const string Save = "api/tenants/upgrade";
    public static string Activate = $"api/tenants/{0}/activate";
    public static string Deactivate = $"api/tenants/{0}/deactivate";
}