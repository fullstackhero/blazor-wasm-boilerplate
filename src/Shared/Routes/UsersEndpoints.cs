namespace FSH.BlazorWebAssembly.Shared.Routes;

public static class UsersEndpoints
{
    public static string GetPermissions(string userId) => $"api/users/{userId}/permissions";
}