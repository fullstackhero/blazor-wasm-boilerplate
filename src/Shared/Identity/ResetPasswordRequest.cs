namespace FSH.BlazorWebAssembly.Shared.Identity;

public class ResetPasswordRequest
{
    public string Email { get; set; }

    public string Password { get; set; }

    public string Token { get; set; }
}