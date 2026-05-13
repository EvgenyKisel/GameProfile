using GameProfile.Core;
using GameProfile.Core.Attributes;

namespace GameProfile.Api.Resources.TesterResource.Request;

public class LoginRequest : HttpRequest
{
    [Body(Name = "login")]
    public string Login { get; set; }

    [Body(Name = "password")]
    public string Password { get; set; }
}
