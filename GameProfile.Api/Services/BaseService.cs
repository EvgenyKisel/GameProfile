using System.Net;
using GameProfile.Utils.Logging;
using RestSharp;
using Xunit;

namespace GameProfile.Api.Services;

public abstract class BaseService(ILogger logger)
{
    protected readonly ILogger Logger = logger;

    public string Token { get; set; }

    protected string ResolveToken(string explicitToken) => explicitToken ?? Token;

    protected static RestResponse<T> ValidateResponse<T>(
        RestResponse<T> response,
        HttpStatusCode expectedStatus = HttpStatusCode.OK)
    {
        Assert.Equal(expectedStatus, response.StatusCode);
        return response;
    }
}
