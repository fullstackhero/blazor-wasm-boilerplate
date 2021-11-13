using FSH.BlazorWebAssembly.Shared.Preference;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Preference
{
    public record ClientPreference : IPreference
    {
        public bool IsDarkMode { get; set; }
        public bool IsRTL { get; set; }
        public bool IsDrawerOpen { get; set; }
        public string PrimaryColor { get; set; } = "#3eaf7c";
        //public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US";
    }
}
