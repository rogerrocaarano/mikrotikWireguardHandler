using Newtonsoft.Json;

namespace routerOSApiHandler;

/// <summary>
/// Defines an object for handling the RouterOS Wireguard Service.
/// </summary>
public class WireguardServer
{
    // Defines the rOSApiClient object used for manage the restAPI requests
    private readonly rOSApiClient _server;
    // Lists all Wireguard Interfaces
    public List<WireguardInterface>? Interfaces { get; set; }
    // Lists all Wireguard Peers
    public List<WireguardPeer>? Peers { get; set; }

    /// <summary>
    /// Creates a WireguardServer object based on a rOSApiClient
    /// </summary>
    /// <param name="server">rOSApiClient Object</param>
    public WireguardServer(rOSApiClient server)
    {
        _server = server;
    }
    
    /// <summary>
    /// Takes in an id as a parameter and returns an object of WireguardInterface type, using the Get method to
    /// retrieve data from an API endpoint that corresponds to the specified id.
    /// </summary>
    /// <param name="rosId">RouterOS id parameter of the interface</param>
    /// <returns>Wireguard Interface object</returns>
    public WireguardInterface GetInterfaceWireguard(string rosId)
    {
        var getIface = Task.Run(async () => await _server.Get("interface/wireguard/" + rosId));
        var jsonString = getIface.Result;
        return JsonConvert.DeserializeObject<WireguardInterface>(jsonString);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="rosId"></param>
    /// <returns></returns>
    public WireguardInterface GetWireguardPeer(string rosId)
    {
        var getIface = Task.Run(async () => await _server.Get("interface/wireguard/peer/" + rosId));
        var jsonString = getIface.Result;
        return JsonConvert.DeserializeObject<WireguardInterface>(jsonString);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public async Task UpdateWireguardInterfaces()
    {
        var jsonString = await _server.Get("interface/wireguard");
        Interfaces = JsonConvert.DeserializeObject<List<WireguardInterface>>(jsonString);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public async Task UpdateWireguardPeers()
    {
        var jsonString = await _server.Get("interface/wireguard/peers");
        Peers = JsonConvert.DeserializeObject<List<WireguardPeer>>(jsonString);
    }
}