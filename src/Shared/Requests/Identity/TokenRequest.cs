using System.ComponentModel.DataAnnotations;

namespace FSH.BlazorWebAssembly.Shared.Requests.Identity
{
    public class TokenRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Tenant { get; set; }
    }
}