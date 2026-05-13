using System.Net;
using GameProfile.Api.Services;
using GameProfile.Core.Configuration;
using GameProfile.Tests.Constants;
using GameProfile.Tests.Utils;
using GameProfile.Utils.Logging;
using Xunit;

namespace GameProfile.Tests.Tests;

[Trait(TraitName.Category, TestCategory.Auth)]
public class TesterLoginTests
{
    private readonly TesterService _testerService = new(new ConsoleLogger(TestContext.Current));

    [Fact]
    public void Tester_Login_200()
    {
        var tester = TestSettingsLoader.Settings.Tester;
        var response = _testerService.Login(tester.Login, tester.Password);

        Assertions.Validate()
            .Equal(HttpStatusCode.OK, response.StatusCode, "Login status code should be 200.")
            .NotNull(response.Data, "Login response body should not be null.")
            .NotNullOrEmpty(response.Data?.Token, "Login response should contain a token field.");
    }
}
