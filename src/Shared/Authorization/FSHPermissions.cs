﻿using System.ComponentModel;

namespace FSH.BlazorWebAssembly.Shared.Authorization;

public class FSHPermissions
{
    [DisplayName("Dashboard")]
    [Description("Dashboard Permissions")]
    public static class Dashboard
    {
        public const string View = "Permissions.Dashboard.View";
    }

    [DisplayName("AuditLogs")]
    [Description("AuditLogs Permissions")]
    public static class AuditLogs
    {
        public const string View = "Permissions.AuditLogs.View";
    }

    [DisplayName("Hangfire")]
    [Description("Hangfire Permissions")]
    public static class Hangfire
    {
        public const string View = "Permissions.Hangfire.View";
    }

    [DisplayName("Identity")]
    [Description("Identity Permissions")]
    public static class Identity
    {
        public const string Register = "Permissions.Identity.Register";
    }

    [DisplayName("Users")]
    [Description("Users Permissions")]
    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Edit = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
        public const string Export = "Permissions.Users.Export";
        public const string Search = "Permissions.Users.Search";
    }

    [DisplayName("Roles")]
    [Description("Roles Permissions")]
    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string ListAll = "Permissions.Roles.ViewAll";
        public const string Register = "Permissions.Roles.Register";
        public const string Update = "Permissions.Roles.Update";
        public const string Remove = "Permissions.Roles.Remove";
    }

    [DisplayName("Products")]
    [Description("Products Permissions")]
    public static class Products
    {
        public const string View = "Permissions.Products.View";
        public const string Search = "Permissions.Products.Search";
        public const string Register = "Permissions.Products.Register";
        public const string Update = "Permissions.Products.Update";
        public const string Remove = "Permissions.Products.Remove";
    }

    [DisplayName("Brands")]
    [Description("Brands Permissions")]
    public static class Brands
    {
        public const string View = "Permissions.Brands.View";
        public const string Search = "Permissions.Brands.Search";
        public const string Register = "Permissions.Brands.Register";
        public const string Update = "Permissions.Brands.Update";
        public const string Remove = "Permissions.Brands.Remove";
    }

    [DisplayName("Role Claims")]
    [Description("Role Claims Permissions")]
    public static class RoleClaims
    {
        public const string View = "Permissions.RoleClaims.View";
        public const string Register = "Permissions.RoleClaims.Register";
        public const string Edit = "Permissions.RoleClaims.Edit";
        public const string Update = "Permissions.RoleClaims.Update";
        public const string Search = "Permissions.RoleClaims.Search";
    }

    [DisplayName("Tenants")]
    [Description("Tenants Permissions")]
    public static class Tenants
    {
        public const string View = "Permissions.Tenants.View";
        public const string ListAll = "Permissions.Tenants.ViewAll";
        public const string Register = "Permissions.Tenants.Register";
        public const string Update = "Permissions.Tenants.Update";
        public const string UpgradeSubscription = "Permissions.Tenants.UpgradeSubscription";
        public const string Remove = "Permissions.Tenants.Remove";
    }
}