using GameProfile.Core.Configuration;

namespace GameProfile.Api;

public static class ApiUrl
{
    private static string BaseUrl => TestSettingsLoader.Settings.ApiBaseUrl;

    public static class TesterResource
    {
        public static string LoginUrl => $"{BaseUrl}/api/tester/login";
    }

    public static class PlayerResource
    {
        private static string Base => $"{BaseUrl}/api/automationTask";

        public static string CreateUrl => $"{Base}/create";
        public static string GetOneUrl => $"{Base}/getOne";
        public static string GetAllUrl => $"{Base}/getAll";
        public static string DeleteOneUrl(string id) => $"{Base}/deleteOne/{id}";
    }
}
