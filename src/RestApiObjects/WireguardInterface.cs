using Newtonsoft.Json;

namespace mikrotikWireguardHandler.RestApiObjects;

/// <summary>
/// Model class of a Wireguard Interface for deserialize from RouterOS API.
/// </summary>
public class WireguardInterface
{
    [JsonProperty(".id")] public string? RosId { get; set; }
    [JsonProperty("disabled")] public string? Disabled { get; set; }
    [JsonProperty("listen-port")] public int? ListenPort { get; set; }
    [JsonProperty("mtu")] public int? Mtu { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("private-key")] public string? PrivateKey { get; set; }
    [JsonProperty("public-key")] public string? PublicKey { get; set; }
    [JsonProperty("running")] public bool? Running { get; set; }
    [JsonProperty("comment")] public string? Comment { get; set; }
}
