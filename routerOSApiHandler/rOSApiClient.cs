namespace routerOSApiHandler;

/// <summary>
/// Defines an object than handles RouterOS RestAPI calls.
/// </summary>
public class rOSApiClient
{
    // Mikrotik RestAPI Server FQDN
    private string restApiServer;
    // URI scheme
    private bool useSSL;
    // Authentication header for basic HTTP authentication
    private System.Net.Http.Headers.AuthenticationHeaderValue AuthHeader;

    /// <summary>
    /// Creates an instance of rosApiClient than uses SSL by default.
    /// </summary>
    /// <param name="serverFqdn">Mikrotik RestAPI Server FQDN</param>
    /// <param name="user">Router's username for RestAPI requests</param>
    /// <param name="password">Router's password for RestAPI requests</param>
    public rOSApiClient(string serverFqdn, string user, string password)
    {
        restApiServer = serverFqdn;
        useSSL = true;
        // generate an authToken for basic HTTP Auth and create a header for the requests.
        var authToken = System.Text.Encoding.UTF8.GetBytes($"{user}:{password}");
        AuthHeader = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
    }
    /// <summary>
    /// Takes a single parameter named "request" and returns a string representing the URI for the RESTful API call.
    /// The "request" parameter is used to build the path part of the URI.
    /// </summary>
    private string RequestUri(string request)
    {
        var scheme = useSSL ? "https://" : "http://";
        return scheme + restApiServer + "/rest/" + request;
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
        apiClient.DefaultRequestHeaders.Authorization = AuthHeader;
        // Send the RESTful API request to the RouterOS server.
        // The response from the RouterOS API is returned as a string.
        var response = await apiClient.GetStringAsync(uri);
        return response;
    }
    
    
}