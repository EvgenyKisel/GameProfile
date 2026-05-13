using GameProfile.Core.Attributes;

namespace GameProfile.Api.Resources.PlayerResource.Request;

public class CreatePlayerRequest : BaseRequest
{
    [Body(Name = "name")]
    public string Name { get; set; }

    [Body(Name = "age")]
    public int? Age { get; set; }

    [Body(Name = "gender")]
    public string Gender { get; set; }

    [Body(Name = "country")]
    public string Country { get; set; }
}
