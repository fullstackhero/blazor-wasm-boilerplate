namespace FSH.BlazorWebAssembly.Shared.Constants
{
    public static class StorageConstants
    {
        public static class Local
        {
            public static string Preference = "clientPreference";

            public static string AuthToken = "authToken";
            public static string RefreshToken = "refreshToken";
            public static string ImageUri = "userImageURL";
        }

        public static class Server
        {
            public static string Preference = "serverPreference";
        }
    }
}
