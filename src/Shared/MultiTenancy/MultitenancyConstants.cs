namespace FSH.BlazorWebAssembly.Shared.MultiTenancy;

public class MultitenancyConstants
{
    public static class DefaultTenant
    {
        public const string Name = "Root";
        public const string Key = "root";
        public const string EmailAddress = "admin@root.com";
    }

    public const string DefaultPassword = "123Pa$$word!";
}
