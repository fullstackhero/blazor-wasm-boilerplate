using FSH.BlazorWebAssembly.Shared.Preference;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Preference
{
    public class ClientPreference : IPreference
    {
        public bool IsDarkMode { get; set; }
        public bool IsRTL { get; set; }
        public bool IsDrawerOpen { get; set; }
        public string PrimaryColor { get; set; } = "#3eaf7c";
    }
}
