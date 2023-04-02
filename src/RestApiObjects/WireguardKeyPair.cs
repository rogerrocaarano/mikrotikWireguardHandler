using Newtonsoft.Json;
namespace mikrotikWireguardHandler.RestApiObjects;

public class WireguardKeyPair
{
    [JsonProperty("public-key")] public string? PublicKey { get; set; }
    [JsonProperty("private-key")] public string? PrivateKey { get; set; }
}