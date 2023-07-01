namespace FL_CRMS_ERP_WASM.Client.Infrastructure.Preferences;

public interface IPreferenceManager : IAppService
{
    Task SetPreference(IPreference preference);

    Task<IPreference> GetPreference();

    Task<bool> ChangeLanguageAsync(string languageCode);
}