namespace routerOSApiHandler;

/// <summary>
/// Defines an object than handles RouterOS RestAPI calls.
/// </summary>
public class rOSApiClient
{
    // Mikrotik RestAPI Server FQDN
    private readonly string _restApiServer;
    // URI scheme and port
    private readonly bool _useSsl;
    private readonly int _portNumber;
    // Authentication header for basic HTTP authentication
    private System.Net.Http.Headers.AuthenticationHeaderValue _authHeader;

    /// <summary>
    /// Creates an instance of rosApiClient. At the moment RouterOS's RestAPI can only be used on www-ssl service.
    /// The parameter ssl can be used to allow non secure connections if Mikrotik allows it in future.
    /// </summary>
    /// <param name="serverFqdn">Mikrotik RestAPI Server FQDN.</param>
    /// <param name="portNumber">Defines the listening port of the RestAPI Server.</param>
    /// <param name="ssl">Defines if the RestAPI client will use an SSL connection.</param>
    /// <param name="user">Router's username for RestAPI requests.</param>
    /// <param name="password">Router's password for RestAPI requests</param>
    public rOSApiClient(string serverFqdn, int portNumber, bool ssl, string user, string password)
    {
        _restApiServer = serverFqdn;
        _useSsl = ssl;
        _portNumber = portNumber;
        // generate an authToken for basic HTTP Auth and create a header for the requests.
        var authToken = System.Text.Encoding.UTF8.GetBytes($"{user}:{password}");
        _authHeader = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
    }
    /// <summary>
    /// Takes a single parameter named "request" and returns a string representing the URI for the RESTful API call.
    /// The "request" parameter is used to build the path part of the URI.
    /// </summary>
    private string RequestUri(string request)
    {
        var scheme = _useSsl ? "https://" : "http://";
        return _useSsl switch
        {
            true when _portNumber == 443 => scheme + _restApiServer + "/rest/" + request,
            false when _portNumber == 80 => scheme + _restApiServer + "/rest/" + request,
            _ => scheme + _restApiServer + ":" + _portNumber + "/rest/" + request
        };
    }
    /// <summary>
    /// takes a single parameter named "request" and returns a string representing the response from the RouterOS API.
    /// The "request" parameter is used to construct the URI for the RESTful API call by calling the private
    /// "RequestUri" method.
    /// </summary>
    public async Task<string> Get(string request)
    {
        // get the full URI for the request:
        var uri = RequestUri(request);
        using var apiClient = new HttpClient();
        apiClient.DefaultRequestHeaders.Authorization = _authHeader;
        // Send the RESTful API request to the RouterOS server.
        // The response from the RouterOS API is returned as a string.
        var response = await apiClient.GetStringAsync(uri);
        return response;
    }
    
    
}