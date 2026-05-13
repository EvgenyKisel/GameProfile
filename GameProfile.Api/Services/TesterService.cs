using GameProfile.Api.Resources.TesterResource;
using GameProfile.Api.Resources.TesterResource.Request;
using GameProfile.Api.Resources.TesterResource.Response;
using GameProfile.Utils.Logging;
using RestSharp;

namespace GameProfile.Api.Services;

public class TesterService(ILogger logger) : BaseService(logger)
{
    private readonly TesterResourceApi _api = new(logger);

    public RestResponse<LoginResponse> Login(string login, string password)
    {
        var request = new LoginRequest
        {
            Login = login,
            Password = password
        };
        return ValidateResponse(_api.Login(request));
    }
}
