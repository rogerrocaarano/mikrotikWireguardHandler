using Newtonsoft.Json;
using routerOSApiHandler.Models;

namespace routerOSApiHandler.Handlers;

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
    /// Requests all Wireguard interfaces from the server and saves it as a list of WireguardInterface
    /// objects in the Interfaces variable.
    /// </summary>
    public async Task UpdateInterfaces()
    {
        var jsonString = await _server.Get("interface/wireguard");
        Interfaces = JsonConvert.DeserializeObject<List<WireguardInterface>>(jsonString);
    }
    
    /// <summary>
    /// Requests all Wireguard peers from the server and saves it as a list of WireguardPeers objects
    /// in the Peers variable.
    /// </summary>
    public async Task UpdatePeers()
    {
        var jsonString = await _server.Get("interface/wireguard/peers");
        Peers = JsonConvert.DeserializeObject<List<WireguardPeer>>(jsonString);
    }

    /// <summary>
    /// Helper function for finding if an interface exists in the Interfaces list.
    /// </summary>
    private bool InterfaceExists(string name)
    {
        return Interfaces.Exists(existingIface => name == existingIface.Name);
    }
    
    /// <summary>
    /// Helper function for serialize a WireguardInterface object removing null values in attributes.
    /// </summary>
    private string SerializeInterface(WireguardInterface iface)
    {
        var json = JsonConvert.SerializeObject(
            iface,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
        );
        return json;
    }
    /// <summary>
    /// Creates a new Interface on the server.
    /// </summary>
    /// <param name="iface">WiregaurdInterface object containing the data to create a new interface.</param>
    public async Task NewInterface(WireguardInterface iface)
    {
        // Verify if the interface name already exists.
        if (InterfaceExists(iface.Name))
        {
            var message = "Interface" + iface.Name + "already exists.";
            Console.WriteLine(message);
            return;
        }
        // Serialize the iface data, ignoring null parameters.
        var json = SerializeInterface(iface);
        // Send the request to the restAPI and update the Interfaces list.
        await _server.Put("interface/wireguard", json);
        await UpdateInterfaces();
    }

    /// <summary>
    /// Updates existing interface parameters.
    /// </summary>
    /// <param name="iface">The interface object with updated parameters.</param>
    public async Task SetInterface(WireguardInterface iface)
    {
        if (InterfaceExists(iface.Name))
        {
            var json = SerializeInterface(iface);
            var requestPath = "interface/wireguard/" + iface.Name;
            await _server.Patch(requestPath, json);
            await UpdateInterfaces();
        }
    }

    /// <summary>
    /// Deletes an existing interface.
    /// </summary>
    /// <param name="iface">The interface object to delete.</param>
    public async Task DeleteInterface(WireguardInterface iface)
    {
        if (InterfaceExists(iface.Name))
        {
            var requestPath = "interface/wireguard/" + iface.Name;
            await _server.Delete(requestPath);
        }
    }
}