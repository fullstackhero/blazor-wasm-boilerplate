namespace FSH.BlazorWebAssembly.Shared.Authorization;

public static class DefaultPermissions
{
    public static Type[] AdminPermissionTypes => typeof(FSHPermissions).GetNestedTypes();

    public static Type[] RootPermissionTypes => typeof(FSHRootPermissions).GetNestedTypes();
}