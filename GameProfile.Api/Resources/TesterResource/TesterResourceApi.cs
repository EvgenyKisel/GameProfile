using GameProfile.Api.Resources.TesterResource.Request;
using GameProfile.Api.Resources.TesterResource.Response;
using GameProfile.Utils.Logging;
using RestSharp;
using HttpClient = GameProfile.Core.HttpClient;

namespace GameProfile.Api.Resources.TesterResource;

public class TesterResourceApi(ILogger logger) : HttpClient(logger)
{
    public RestResponse<LoginResponse> Login(LoginRequest request) =>
        SendPostRequest<LoginResponse>(ApiUrl.TesterResource.LoginUrl, request);
}
