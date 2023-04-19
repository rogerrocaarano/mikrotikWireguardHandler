### Project description

This library provides functions to handle the Wireguard Service on a 
Mikrotik Router.

### File structure description

- `mikrotikApiClient.cs` is a client for the Restfull API, it provides the methods 
Get, Patch, Put and Delete needed for API consumption.
- `WireguardServer.cs` Provides methods for handling:
  - Wireguard Interfaces.
  - Wireguard Peers.
  - IP Addresses of the interfaces.
- Objects in `RestApiObjects` folder are templates used for serialize/deserialize 
data from the router.

###  Example usage

```
# Include the library
using mikrotikWireguardHandler;
using mikrotikWireguardHandler.RestApiObjects;

# create a new api client with your routers credentials:
var apiClient = new mikrotikApiClient(
  "router.example.com", // router FQDN
  443,                      // web-ssl service port
  true,                     // Use SSL
  "apiUser",                // Username
  "superSecurePass"         // Password
 );

# Create WireguardServer object with apiClient data:
var router = new WireguardServer(apiClient);

# Update all data and print all wireguard interfaces names:
await router.Update();
foreach (var iface in server.Interfaces)
{
    Console.WriteLine(iface.Name);
}
```