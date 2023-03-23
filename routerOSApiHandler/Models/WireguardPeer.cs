using Newtonsoft.Json;

namespace routerOSApiHandler.Models;

/// <summary>
/// Model class of a Wireguard Peer for deserialize from RouterOS API.
/// </summary>
public class WireguardPeer
{
    [JsonProperty(".id")] public string Id { get; set; }
    [JsonProperty("allowed-address")] public string AllowedAddress { get; set; }
    [JsonProperty("comment")] public string Comment { get; set; }
    [JsonProperty("current-endpoint-address")] public string CurrentEndpointAddress { get; set; }
    [JsonProperty("current-endpoint-port")] public string CurrentEndpointPort { get; set; }
    [JsonProperty("disabled")] public string Disabled { get; set; }
    [JsonProperty("endpoint-address")] public string EndpointAddress { get; set; }
    [JsonProperty("endpoint-port")] public string EndpointPort { get; set; }
    [JsonProperty("interface")] public string Interface { get; set; }
    [JsonProperty("last-handshake")] public string LastHandshake { get; set; }
    [JsonProperty("public-key")] public string PublicKey { get; set; }
    [JsonProperty("rx")] public string Rx { get; set; }
    [JsonProperty("tx")] public string Tx { get; set; }
}