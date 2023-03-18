namespace routerOSApiHandler;

public class rOSApiClient
{
    protected string restApiServer { get; set; }
    protected string restApiUser { get; set; }
    protected string restApiPassword { get; set; }
    protected bool useSSL { get; set; }
    
    public rOSApiClient(string serverFqdn, string user, string password)
    {
        restApiServer = serverFqdn;
        restApiUser = user;
        restApiPassword = password;
        useSSL = true;
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
    /// The "request" parameter is used to construct the URI for the RESTful API call by calling the private "RequestUri" method.
    /// </summary>
    public async Task<string> Get(string request)
    {
        // get the full URI for the request:
        var uri = RequestUri(request);
        // create a new instance of the HttpClient and set the Authorization header of the HTTP request to
        // the value "Basic" followed by the Base64-encoded authentication token. This authenticates the request
        // with the RouterOS API using the username and password provided in the constructor.
        using var apiClient = new HttpClient();
        var authToken = System.Text.Encoding.UTF8.GetBytes($"{restApiUser}:{restApiPassword}");  
        apiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        // Send the RESTful API request to the RouterOS server.
        // The response from the RouterOS API is returned as a string.
        var test = await apiClient.GetStringAsync(uri);
        return test;
    }
}