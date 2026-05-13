namespace GameProfile.Core.Configuration;

public sealed record TestSettings
{
    public string Environment { get; init; } = "Dev";
    public string ApiBaseUrl { get; init; } = "";
    public TesterCredentials Tester { get; init; } = new();
}

public sealed class TesterCredentials
{
    public string Login { get; init; } = "";
    public string Password { get; init; } = "";
}
