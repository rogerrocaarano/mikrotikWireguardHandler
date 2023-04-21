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
    
    private bool InterfaceExists(WireguardInterface iface)
    {
        return Interfaces != null && Interfaces.Exists(existingIface => iface.Name == existingIface.Name);
    }
    
    /// <summary>
    /// Helper function for finding if a peer exists in the Peers list.
    /// </summary>
    private bool PeerExists(WireguardPeer peer)
    {
        return Peers != null && Peers.Exists(existingPeer => peer.PublicKey == existingPeer.PublicKey);
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
        if (routerAddresses != null)
            foreach (var address in routerAddresses)
            {
                if (Interfaces != null &&
                    Interfaces.Exists(wireguardInterface => address.Interface == wireguardInterface.Name))
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
        // Verify if the interface already exists.
        if (iface.Name != null && InterfaceExists(iface))
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
        if (InterfaceExists(iface))
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
        if (InterfaceExists(iface))
        {
            var requestPath = "interface/wireguard/" + iface.Name;
            await _server.Delete(requestPath);
            // Delete addresses associated with the interface
            if (Addresses != null)
                foreach (var address in Addresses.Where(address => address.Interface == iface.Name))
                {
                    await DeleteAddress(address);
                }

            // Delete all peers associated with the interface
            if (Peers != null)
                foreach (var peer in Peers.Where(peer => peer.Interface == iface.Name))
                {
                    await DeletePeer(peer);
                }
        }
    }
    
    /// <summary>
    /// Deletes an ip address from the server.
    /// </summary>
    /// <param name="address">IpAddress object than contains an RosId used to delete it from the server.</param>
    public async Task DeleteAddress(IpAddress address)
    {
        var requestPath = "ip/address/" + address.RosId;
        await _server.Delete(requestPath);
    }
    
    /// <summary>
    /// Creates a new Peer in the server.
    /// </summary>
    /// <param name="peer">WireguardPeer object containing the data to create a new peer.</param>
    public async Task NewPeer(WireguardPeer peer)
    {
        if (PeerExists(peer))
        {
            var message = "Peer " + peer.PublicKey + " already exists.";
            Console.WriteLine(message);
            return;
        }
        // If no public key is provided, generate a keypair.
        if (peer.PublicKey == null)
        {
            var keys = await GenerateKeys();
            peer.PublicKey = keys.PublicKey;
            peer.PrivateKey = keys.PrivateKey;
        }
        // Serialize the peer data, ignoring null parameters.
        var json = SerializeIgnoringNull(peer);
        // Send the request to the restAPI and update the Peers list.
        await _server.Put("interface/wireguard/peer", json);
    }

    /// <summary>
    /// Updates existing peer parameters.
    /// </summary>
    /// <param name="peer">The peer object with updated parameters.</param>
    public async Task SetPeer(WireguardPeer peer)
    {
        if (PeerExists(peer))
        {
            var json = SerializeIgnoringNull(peer);
            var requestPath = "interface/wireguard/peer/" + peer.RosId;
            await _server.Patch(requestPath, json);
        }
    }
    
    /// <summary>
    /// Deletes a peer from the server.
    /// </summary>
    /// <param name="peer">WireguardPeer object than contains an RosId used to delete it from the server.</param>
    public async Task DeletePeer(WireguardPeer peer)
    {
        var requestPath = "interface/wireguard/peer/" + peer.RosId;
        await _server.Delete(requestPath);
    }

    /// <summary>
    /// Generates a private and public 64-base encoded key pair, using NewInterface() method as a generator.
    /// It's not a proper way to do it, but it gets the job done.
    /// </summary>
    /// <returns>WireguardKeyPair object.</returns>
    public async Task<WireguardKeyPair> GenerateKeys()
    {
        // Create a dummy interface for generating keys.
        var keyGenInterface = new WireguardInterface();
        keyGenInterface.Name = "keyGen";
        await NewInterface(keyGenInterface);
        await UpdateInterfaces();
        keyGenInterface = Interfaces.Find(iface => iface.Name.Equals("keyGen"));
        // Get keyPair from the generated interface.
        var keyPair = new WireguardKeyPair();
        keyPair.PublicKey = keyGenInterface.PublicKey;
        keyPair.PrivateKey = keyGenInterface.PrivateKey;
        await DeleteInterface(keyGenInterface);
        await UpdateInterfaces();
        return keyPair;
    }
}