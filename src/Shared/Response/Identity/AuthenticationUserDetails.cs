namespace FSH.BlazorWebAssembly.Shared.Response.Identity
{
    public class AuthenticationUserDetails
    {
        public AuthenticationUserDetails(string id, string userName, string fullName, string email, string tenant)
        {
            Id = id;
            UserName = userName;
            FullName = fullName;
            Email = email;
            Tenant = tenant;
        }
        public string Id { get; set; }
        public string UserName {  get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Tenant { get; set; }
    }
}
