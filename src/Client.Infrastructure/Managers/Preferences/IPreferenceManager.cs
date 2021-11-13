using FSH.BlazorWebAssembly.Shared.Preference;
using FSH.BlazorWebAssembly.Shared.Wrapper;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Managers.Preferences
{
    public interface IPreferenceManager : IManager
    {
        Task SetPreference(IPreference preference);

        Task<IPreference> GetPreference();

        Task<IResult> ChangeLanguageAsync(string languageCode);
    }
}
