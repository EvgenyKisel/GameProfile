using GameProfile.Utils.Logging;

namespace GameProfile.Api.Services;

public abstract class BaseService(ILogger logger)
{
    protected readonly ILogger Logger = logger;

    public string Token { get; set; }

    protected string ResolveToken(string explicitToken)
    {
        var token = explicitToken ?? Token;
        if (string.IsNullOrEmpty(token))
            throw new InvalidOperationException(
                "No auth token available. Set Token on the service or pass one explicitly.");
        return token;
    }
}
