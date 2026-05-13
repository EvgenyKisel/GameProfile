using GameProfile.Core.Attributes;

namespace GameProfile.Api.Resources.PlayerResource.Request;

public class GetPlayerRequest : BaseRequest
{
    [UrlParameter(Name = "id")]
    public long? Id { get; set; }
}
