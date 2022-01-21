namespace FSH.BlazorWebAssembly.Shared.Authorization;

public static class DefaultPermissions
{
    public static List<string> Admin => typeof(FSHPermissions).GetNestedClassesStaticStringValues();

    public static List<string> Root => typeof(FSHRootPermissions).GetNestedClassesStaticStringValues();
}