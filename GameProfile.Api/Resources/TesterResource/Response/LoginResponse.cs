using Newtonsoft.Json;

namespace GameProfile.Api.Resources.TesterResource.Response;

public class LoginResponse
{
    [JsonProperty("token")]
    public string Token { get; set; }
}
