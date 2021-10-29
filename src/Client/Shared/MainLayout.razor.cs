using FSH.BlazorWebAssembly.Client.Theme;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Shared
{
    public partial class MainLayout
    {
        private MudTheme _theme = new LightTheme();

        [Parameter]
        public EventCallback OnDarkModeToggle { get; set; }

        public bool _drawerOpen = true;

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        protected override void OnInitialized()
        {
            StateHasChanged();
        }

        private List<BreadcrumbItem> _items = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Dashboard", href: "/"),
        };
        public async Task ToggleDarkMode()
        {
            _theme = new DarkTheme();
        }
    }
}
