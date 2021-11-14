using FSH.BlazorWebAssembly.Shared.Preference;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Preference
{
    public class ClientPreference : IPreference
    {
        public bool IsDarkMode { get; set; } = false;
        public bool IsRTL { get; set; } = false;
        public bool IsDrawerOpen { get; set; } = false;
        public string PrimaryColor { get; set; } = "#3eaf7c";
        public string SecondaryColor { get; set; } = "#2196f3";
        public double BorderRadius { get; set; } = 5;
    }
}
