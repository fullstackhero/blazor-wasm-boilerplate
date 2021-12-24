using System.Security.Claims;

namespace FSH.BlazorWebAssembly.Shared.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.Email);

    public static string? GetTenant(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(FSHClaims.Tenant);

    public static string? GetFullName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal?.FindFirst(FSHClaims.Fullname)?.Value;

    public static string? GetFirstName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal?.FindFirst(ClaimTypes.Name)?.Value;

    public static string? GetSurname(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal?.FindFirst(ClaimTypes.Surname)?.Value;

    public static string? GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);

    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
       => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

    public static string? GetImageUrl(this ClaimsPrincipal claimsPrincipal)
       => claimsPrincipal.FindFirstValue(FSHClaims.ImageUrl);

    public static DateTimeOffset GetExpiration(this ClaimsPrincipal claimsPrincipal) =>
        DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(
            claimsPrincipal.FindFirstValue(FSHClaims.Expiration)));
}