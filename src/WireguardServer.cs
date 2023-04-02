using mikrotikWireguardHandler.RestApiObjects;
using Newtonsoft.Json;

namespace mikrotikWireguardHandler;

/// <summary>
/// Defines an object for handling the RouterOS Wireguard Service.
/// </summary>
public class WireguardServer
{
    // Defines the mikrotikApiClient object used for manage the restAPI requests
    private readonly mikrotikApiClient _server;
    // Lists all Wireguard Interfaces
    public List<WireguardInterface>? Interfaces { get; set; }
    // Lists all Wireguard Peers
    public List<WireguardPeer>? Peers { get; set; }
    public List<IpAddress>? Addresses { get; set; }

    /// <summary>
    /// Creates a WireguardServer object based on a mikrotikApiClient
    /// </summary>
    /// <param name="server">mikrotikApiClient Object</param>
    public WireguardServer(mikrotikApiClient server)
    {
        _server = server;
    }

    /// <summary>
    /// Helper function for finding if an interface exists in the Interfaces list.
    /// </summary>
    
    private bool InterfaceExists(string name)
    {
        return Interfaces.Exists(existingIface => name == existingIface.Name);
    }
    
    //todo: Documentation.
    private bool PeerExists(WireguardPeer peer)
    {
        return Peers.Exists(existingPeer => peer.PublicKey == existingPeer.PublicKey);
    }
    
    /// <summary>
    /// Helper function for serialize a WireguardInterface object removing null values in attributes.
    /// </summary>
    private string SerializeIgnoringNull(Object toSerialize)
    {
        var json = JsonConvert.SerializeObject(
            toSerialize,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
        );
        return json;
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
    /// Request all IP Addresses than belongs to a Wireguard interface on the server and saves it as a list of
    /// IpAddress objects.
    /// </summary>
    public async Task UpdateAddresses()
    {
        var jsonString = await _server.Get("ip/address");
        var routerAddresses = JsonConvert.DeserializeObject<List<IpAddress>>(jsonString);
        await UpdateInterfaces();
        var validAddress = new List<IpAddress>();
        foreach (var address in routerAddresses)
        {
            if (Interfaces.Exists(wireguardInterface => address.Interface == wireguardInterface.Name))
            {
                validAddress.Add(address);
            }
        }
        Addresses = validAddress;
    }

    /// <summary>
    /// Request all server information such as interfaces, addresses and peers.
    /// </summary>
    public async Task Update()
    {
        await UpdateInterfaces();
        await UpdateAddresses();
        await UpdatePeers();
    }
    
    /// <summary>
    /// Creates a new Interface on the server.
    /// </summary>
    /// <param name="iface">WireguardInterface object containing the data to create a new interface.</param>
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
        var json = SerializeIgnoringNull(iface);
        Console.WriteLine(json);
        // Send the request to the restAPI and update the Interfaces list.
        await _server.Put("interface/wireguard", json);
    }

    /// <summary>
    /// Updates existing interface parameters.
    /// </summary>
    /// <param name="iface">The interface object with updated parameters.</param>
    public async Task SetInterface(WireguardInterface iface)
    {
        if (InterfaceExists(iface.Name))
        {
            var json = SerializeIgnoringNull(iface);
            var requestPath = "interface/wireguard/" + iface.Name;
            await _server.Patch(requestPath, json);
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
            // Delete addresses associated with the interface
            foreach (var address in Addresses.Where(address => address.Interface == iface.Name))
            {
                await DeleteAddress(address);
            }
            // Delete all peers associated with the interface
            foreach (var peer in Peers.Where(peer => peer.Interface == iface.Name))
            {
                await DeletePeer(peer);
            }
        }
    }
    
    /// <summary>
    /// Deletes an ip address from the server.
    /// </summary>
    /// <param name="address">IpAddress object than contains an Id used to delete it from the server.</param>
    public async Task DeleteAddress(IpAddress address)
    {
        var requestPath = "ip/address/" + address.Id;
        await _server.Delete(requestPath);
    }
    
    public async Task NewPeer(WireguardPeer peer)
    {
        if (PeerExists(peer))
        {
            var message = "Peer " + peer.PublicKey + " already exists.";
            Console.WriteLine(message);
            return;
        }
        // Serialize the peer data, ignoring null parameters.
        var json = SerializeIgnoringNull(peer);
        // Send the request to the restAPI and update the Peers list.
        await _server.Put("interface/wireguard/peer", json);
    }

    public async Task SetPeer(WireguardPeer peer)
    {
        if (PeerExists(peer))
        {
            var json = SerializeIgnoringNull(peer);
            var requestPath = "interface/wireguard/peer/" + peer.Id;
            await _server.Patch(requestPath, json);
        }
    }
    public async Task DeletePeer(WireguardPeer peer)
    {
        var requestPath = "interface/wireguard/peer/" + peer.Id;
        await _server.Delete(requestPath);
    }
}