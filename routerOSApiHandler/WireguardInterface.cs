using Newtonsoft.Json;

namespace routerOSApiHandler;

/// <summary>
/// Model class of a Wireguard Interface for deserialize from RouterOS API.
/// </summary>
public class WireguardInterface
{
    [JsonProperty(".id")] public string Id { get; set; }
    [JsonProperty("disabled")] public string Disabled { get; set; }
    [JsonProperty("listen-port")] public int ListenPort { get; set; }
    [JsonProperty("mtu")] public int Mtu { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("private-key")] public string PrivateKey { get; set; }
    [JsonProperty("public-key")] public string PublicKey { get; set; }
    [JsonProperty("running")] public string Running { get; set; }
}
