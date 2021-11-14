using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Components.ThemeManager
{
    public partial class ColorPanel
    {
        [Parameter]
        public List<string> Colors { get; set; } = new();
        [Parameter]
        public string ColorType { get; set; } = string.Empty;
        [Parameter]
        public Color CurrentColor { get; set; }
        [Parameter]
        public EventCallback<string> OnColorClicked { get; set; }

        protected async Task ColorClicked(string color)
        {
            await OnColorClicked.InvokeAsync(color);
        }
    }
}