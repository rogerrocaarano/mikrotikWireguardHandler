using System.Text;

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
    /// Gets a full URI based on a RouterOS requestPath string.
    /// <param name="requestPath">Path to desired object in RouterOS without RestAPI URL.</param>
    /// <returns>Full URI of an RouterOS object.</returns>
    /// </summary>
    private string RequestUri(string requestPath)
    {
        var scheme = _useSsl ? "https://" : "http://";
        return _useSsl switch
        {
            true when _portNumber == 443 => scheme + _restApiServer + "/rest/" + requestPath,
            false when _portNumber == 80 => scheme + _restApiServer + "/rest/" + requestPath,
            _ => scheme + _restApiServer + ":" + _portNumber + "/rest/" + requestPath
        };
    }
    /// <summary>
    /// Request an object from the RestAPI server.
    /// <param name="requestPath">Path to desired object in RouterOS without RestAPI URL.</param>
    /// <returns>A string representing the response from the RouterOS API.</returns>
    /// </summary>
    public async Task<string> Get(string requestPath)
    {
        // get the full URI for the requestPath:
        var uri = RequestUri(requestPath);
        using var apiClient = new HttpClient();
        apiClient.DefaultRequestHeaders.Authorization = _authHeader;
        // Send the RESTful API requestPath to the RouterOS server.
        // The response from the RouterOS API is returned as a string.
        var response = await apiClient.GetStringAsync(uri);
        return response;
    }
    
    /// <summary>
    /// Sends a PATCH request to the server for updating records.
    /// </summary>
    /// <param name="requestPath">Path to desired object in RouterOS without RestAPI URL.</param>
    /// <param name="jsonData">A Json object containing the updated parameters for the record.</param>
    public async Task Patch(string requestPath, string jsonData)
    {
        using var apiClient = new HttpClient();
        apiClient.DefaultRequestHeaders.Authorization = _authHeader;
        var requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var requestUri = RequestUri(requestPath);
        await apiClient.PatchAsync(requestUri, requestContent);
    }
    
    /// <summary>
    /// Sends a DELETE request to the server for updating records.
    /// </summary>
    /// <param name="requestPath">Path to desired object in RouterOS without RestAPI URL.</param>
    /// <param name="jsonData">A JSON object containing parameters for a new record.</param>
    public async Task Put(string requestPath, string jsonData)
    {
        using var apiClient = new HttpClient();
        apiClient.DefaultRequestHeaders.Authorization = _authHeader;
        var requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var requestUri = RequestUri(requestPath);
        await apiClient.PutAsync(requestUri, requestContent);
    }
    
    /// <summary>
    /// Sends a DELETE request to the server for deleting records.
    /// </summary>
    /// <param name="requestPath">Path to desired object in RouterOS without RestAPI URL.</param>
    public async Task Delete(string requestPath)
    {
        using var apiClient = new HttpClient();
        apiClient.DefaultRequestHeaders.Authorization = _authHeader;
        var requestUri = RequestUri(requestPath);
        await apiClient.DeleteAsync(requestUri);
    }
}