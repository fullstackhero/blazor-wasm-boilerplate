using FSH.BlazorWebAssembly.Client.Infrastructure.Extensions;
using Microsoft.AspNetCore.Components;

namespace FSH.BlazorWebAssembly.Client.Shared
{
    public partial class PersonCard
    {
        [Parameter] public string? Class { get; set; }
        [Parameter] public string? Style { get; set; }

        private string? UserId { get; set; }
        private string? Email { get; set; }
        private string? FullName { get; set; }
        private string? ImageUri { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadUserData();
            }
        }

        private async Task LoadUserData()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;
            if (user.Identity?.IsAuthenticated == true)
            {
                if (string.IsNullOrEmpty(UserId))
                {
                    FullName = user.GetName();
                    UserId = user.GetUserId();
                    Email = user.GetEmail();
                    StateHasChanged();
                }
            }
        }
    }
}
