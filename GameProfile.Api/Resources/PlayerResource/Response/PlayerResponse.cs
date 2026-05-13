using Newtonsoft.Json;

namespace GameProfile.Api.Resources.PlayerResource.Response;

public class PlayerResponse
{
    [JsonProperty("id")]
    public long? Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("age")]
    public int? Age { get; set; }

    [JsonProperty("gender")]
    public string Gender { get; set; }

    [JsonProperty("country")]
    public string Country { get; set; }
}
