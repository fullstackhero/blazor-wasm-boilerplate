﻿using System.ComponentModel;

namespace FSH.BlazorWebAssembly.Shared.Authorization;

public class FSHRootPermissions
{

    [DisplayName("Tenants")]
    [Description("Tenants Permissions")]
    public static class Tenants
    {
        public const string View = "Permissions.Root.Tenants.View";
        public const string Create = "Permissions.Root.Tenants.Create";
        public const string Update = "Permissions.Root.Tenants.Update";
        public const string UpgradeSubscription = "Permissions.Root.Tenants.UpgradeSubscription";
        public const string Delete = "Permissions.Root.Tenants.Delete";
    }
}