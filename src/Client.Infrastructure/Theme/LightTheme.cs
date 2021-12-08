using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Infrastructure.Theme;

public class LightTheme : MudTheme
{
    public LightTheme()
    {
        Palette = new Palette()
        {
            Primary = CustomColors.Light.Primary,
            Secondary = CustomColors.Light.Secondary,
            Background = CustomColors.Light.Background,
            AppbarBackground = CustomColors.Light.AppbarBackground,
            AppbarText = CustomColors.Light.AppbarText,
            DrawerBackground = CustomColors.Light.Background,
            DrawerText = "rgba(0,0,0, 0.7)",
            Success = CustomColors.Light.Primary,
            TableLines = "#e0e0e029",
            OverlayDark = "hsl(0deg 0% 0% / 75%)"
        };
        LayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "5px"
        };

        Typography = CustomTypography.FSHTypography;
        Shadows = new Shadow();
        ZIndex = new ZIndex() { Drawer = 1300 };
    }
}