using Newtonsoft.Json;
namespace mikrotikWireguardHandler.RestApiObjects;

public class IpAddress
{
    [JsonProperty(".id")] public string? RosId { get; set; }
    [JsonProperty("address")] public string? Address { get; set; }
    [JsonProperty("disabled")] public bool? Disabled { get; set; }
    [JsonProperty("interface")] public string? Interface { get; set; }
    [JsonProperty("network")] public string? Network { get; set; }
}