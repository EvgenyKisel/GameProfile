using GameProfile.Core;
using GameProfile.Core.Attributes;

namespace GameProfile.Api.Resources;

public class BaseRequest : HttpRequest
{
    [Header(Name = "Authorization")]
    public string Authorization { get; set; }
}
