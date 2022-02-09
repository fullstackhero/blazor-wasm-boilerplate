using FSH.BlazorWebAssembly.Client.Infrastructure.Theme;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Preferences;

public class ClientPreference : IPreference
{
    public bool IsDarkMode { get; set; }
    public bool IsRTL { get; set; }
    public bool IsDrawerOpen { get; set; }
    public string PrimaryColor { get; set; } = CustomColors.Light.Primary;
    public string SecondaryColor { get; set; } = CustomColors.Light.Secondary;
    public double BorderRadius { get; set; } = 5;
    public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US";
    public EntityTablePreference EntityTablePreference { get; set; } = new EntityTablePreference();
}
