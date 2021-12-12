namespace FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static string? GetEmail(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.Email);

    public static string? GetTenant(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.Name); // Todo: ?? think this is not right

    public static string? GetName(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal?.FindFirst("fullName")?.Value;

    public static string? GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);

    public static string? GetUserId(this ClaimsPrincipal claimsPrincipal)
       => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

    public static string? GetImageUrl(this ClaimsPrincipal claimsPrincipal)
       => claimsPrincipal.FindFirstValue("image_url");
}