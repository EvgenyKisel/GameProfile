using System.Net;
using GameProfile.Api.Services;
using GameProfile.Core.Configuration;
using GameProfile.Utils.Logging;

namespace GameProfile.Tests.Fixtures;

public class LoginFixture
{
    public string Token { get; }

    public LoginFixture()
    {
        var logger = new ConsoleLogger();
        var settings = TestSettingsLoader.Settings;
        logger.Log($"LoginFixture: logging in for {settings.Environment} ({settings.ApiBaseUrl}).");

        var response = new TesterService(logger).Login(settings.Tester.Login, settings.Tester.Password);

        if (response.StatusCode != HttpStatusCode.OK)
            throw new InvalidOperationException(
                $"Login failed: {(int)response.StatusCode} {response.StatusCode}. Body: {response.Content}");

        Token = response.Data?.Token;
        if (string.IsNullOrEmpty(Token))
            throw new InvalidOperationException(
                "Login returned 200 but no token; cannot authenticate downstream requests.");
    }
}
