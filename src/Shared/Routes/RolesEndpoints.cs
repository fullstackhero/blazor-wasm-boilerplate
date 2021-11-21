namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Routes
{
    public static class RolesEndpoints
    {
        public static string Delete = "api/roles";
        public static string GetAll = "api/roles/all";
        public static string Save = "api/roles";

        public static string GetPermissions = "api/identity/role/permissions/";
        public static string UpdatePermissions = "api/identity/role/permissions/update";
    }
}