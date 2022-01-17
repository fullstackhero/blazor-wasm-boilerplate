using FSH.BlazorWebAssembly.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSH.BlazorWebAssembly.Shared.Authorization;
public static class DefaultPermissions
{
    public static List<string> Admin => typeof(FSHPermissions).GetNestedClassesStaticStringValues();

    public static List<string> Root => typeof(FSHRootPermissions).GetNestedClassesStaticStringValues();
}
