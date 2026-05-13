using GameProfile.Api.Resources.PlayerResource.Request;
using GameProfile.Api.Resources.PlayerResource.Response;
using GameProfile.Utils.Logging;
using RestSharp;
using HttpClient = GameProfile.Core.HttpClient;

namespace GameProfile.Api.Resources.PlayerResource;

public class PlayerResourceApi(ILogger logger) : HttpClient(logger)
{
    public RestResponse<PlayerResponse> CreatePlayer(CreatePlayerRequest request) =>
        SendPostRequest<PlayerResponse>(ApiUrl.PlayerResource.CreateUrl, request);

    public RestResponse<PlayerResponse> GetPlayer(GetPlayerRequest request) =>
        SendGetRequest<PlayerResponse>(ApiUrl.PlayerResource.GetOneUrl, request);

    public RestResponse<List<PlayerResponse>> GetAllPlayers(GetAllPlayersRequest request) =>
        SendGetRequest<List<PlayerResponse>>(ApiUrl.PlayerResource.GetAllUrl, request);

    public RestResponse<object> DeletePlayer(DeletePlayerRequest request, string id) =>
        SendDeleteRequest<object>(ApiUrl.PlayerResource.DeleteOneUrl(id), request);
}
