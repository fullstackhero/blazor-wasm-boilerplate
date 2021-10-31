using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Shared.Routes
{
    public static class TokenEndpoints
    {
        public static string AuthenticationEndpoint = "api/tokens";
        public static string Refresh = "api/identity/token/refresh";
    }
}
