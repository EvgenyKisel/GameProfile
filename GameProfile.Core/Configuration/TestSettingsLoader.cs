using DotNetEnv;
using Microsoft.Extensions.Configuration;

namespace GameProfile.Core.Configuration;

// Layered config (low to high): appsettings.{env}.json, .env credentials,
// GAMEPROFILE_* env vars. Environment defaults to "Dev"; override with
// ENVIRONMENT (.env) or GAMEPROFILE_Environment (shell/CI).
public static class TestSettingsLoader
{
    private const string EnvVarPrefix = "GAMEPROFILE_";
    private const string PrefixedEnvironmentVar = EnvVarPrefix + "Environment";
    private const string ShortEnvironmentVar = "ENVIRONMENT";

    private static readonly Lazy<TestSettings> _settings = new(Load);

    public static TestSettings Settings => _settings.Value;

    private static TestSettings Load()
    {
        var dir = AppContext.BaseDirectory;

        var envPath = Path.Combine(dir, ".env");
        if (File.Exists(envPath))
        {
            Env.Load(envPath, new LoadOptions(clobberExistingVars: false));
        }

        var environment =
            System.Environment.GetEnvironmentVariable(PrefixedEnvironmentVar)
            ?? System.Environment.GetEnvironmentVariable(ShortEnvironmentVar)
            ?? "Dev";

        // Map short .env keys (TESTER_LOGIN, TESTER_PASSWORD) onto Tester:Login / Tester:Password
        // so callers can keep .env terse instead of using the GAMEPROFILE_Tester__* form.
        var credentialOverrides = new Dictionary<string, string>();
        var login = System.Environment.GetEnvironmentVariable("TESTER_LOGIN");
        if (!string.IsNullOrWhiteSpace(login)) credentialOverrides["Tester:Login"] = login;
        var password = System.Environment.GetEnvironmentVariable("TESTER_PASSWORD");
        if (!string.IsNullOrWhiteSpace(password)) credentialOverrides["Tester:Password"] = password;

        var config = new ConfigurationBuilder()
            .SetBasePath(dir)
            .AddJsonFile($"appsettings.{environment.ToLowerInvariant()}.json", optional: true, reloadOnChange: false)
            .AddInMemoryCollection(credentialOverrides)
            .AddEnvironmentVariables(prefix: EnvVarPrefix)
            .Build();

        var settings = config.Get<TestSettings>()
            ?? throw new InvalidOperationException("Failed to bind TestSettings from configuration.");

        return settings with
        {
            Environment = environment,
            ApiBaseUrl = settings.ApiBaseUrl?.TrimEnd('/') ?? ""
        };
    }
}
