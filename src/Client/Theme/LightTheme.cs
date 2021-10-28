using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Theme
{
    public class LightTheme : MudTheme
    {

        public LightTheme()
        {
            Palette = new Palette()
            {
                Primary = CustomColors.FSH.Primary,
                Secondary = Colors.DeepPurple.Accent2,
                Background = CustomColors.FSH.White,
                AppbarBackground = Colors.Blue.Darken1,
                DrawerBackground = CustomColors.FSH.White,
                DrawerText = "rgba(0,0,0, 0.7)",
                Success = "#06d79c"
            };

            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = "2px",
            };

            Typography = CustomTypography.FSHTypography;
            Shadows = new Shadow();
            ZIndex = new ZIndex();
        }
    }
}
