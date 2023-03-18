namespace routerOSApiHandler;

public class rOSApiClient
{
    protected string restApiServer { get; set; }
    protected string restApiUser { get; set; }
    protected string restApiPassword { get; set; }
    protected bool useSSL { get; set; }

    // Constructors
    public rOSApiClient(string serverFqdn, string user, string password)
    {
        restApiServer = serverFqdn;
        restApiUser = user;
        restApiPassword = password;
        useSSL = true;
    }
    private string RequestUri(string request)
    {
        var scheme = useSSL ? "https://" : "http://";
        return scheme + restApiServer + "/rest/" + request;
    }

    public async Task<string> getCall(string request)
    {
        var uri = RequestUri(request);

        using var apiClient = new HttpClient();
        var authToken = System.Text.Encoding.UTF8.GetBytes($"{restApiUser}:{restApiPassword}");  
        apiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

        var test = await apiClient.GetStringAsync(uri);

        return test;
    }
}