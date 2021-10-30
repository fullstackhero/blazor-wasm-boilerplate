using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Theme
{
    public class LightTheme : MudTheme
    {
        public LightTheme()
        {
            Palette = new Palette()
            {
                Primary = CustomColors.Light.Primary,
                Secondary = Colors.DeepPurple.Accent2,
                Background = CustomColors.Light.Background,
                AppbarBackground = CustomColors.Light.Primary,
                DrawerBackground = CustomColors.Light.Background,
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
