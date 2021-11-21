using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Shared.Response
{
    public class StatsDto
    {
        public int ProductCount { get; set; }
        public int BrandCount { get; set; }
        public int UserCount { get; set; }
        public int RoleCount { get; set; }
    }
}
