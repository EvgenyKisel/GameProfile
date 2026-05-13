using GameProfile.Api.Resources.PlayerResource;
using GameProfile.Api.Resources.PlayerResource.Request;
using GameProfile.Api.Resources.PlayerResource.Response;
using GameProfile.Utils.Logging;
using RestSharp;

namespace GameProfile.Api.Services;

public class AutomationTaskService(ILogger logger) : BaseService(logger)
{
    private readonly PlayerResourceApi _api = new(logger);

    public RestResponse<PlayerResponse> CreatePlayer(
        string name,
        int? age,
        string gender,
        string country,
        string token = null) =>
        _api.CreatePlayer(new CreatePlayerRequest
        {
            Authorization = ResolveToken(token),
            Name = name,
            Age = age,
            Gender = gender,
            Country = country
        });

    public RestResponse<PlayerResponse> GetPlayer(long id, string token = null) =>
        _api.GetPlayer(new GetPlayerRequest
        {
            Authorization = ResolveToken(token),
            Id = id
        });

    public RestResponse<List<PlayerResponse>> GetAllPlayers(string token = null) =>
        _api.GetAllPlayers(new GetAllPlayersRequest
        {
            Authorization = ResolveToken(token)
        });

    public RestResponse<object> DeletePlayer(long id, string token = null) =>
        _api.DeletePlayer(
            new DeletePlayerRequest { Authorization = ResolveToken(token) },
            id.ToString());
}
