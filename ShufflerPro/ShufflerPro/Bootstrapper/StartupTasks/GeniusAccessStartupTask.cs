using System.Net.Http;
using System.Text;
using Bootstrap.Extensions.StartupTasks;
using Newtonsoft.Json.Linq;
using ShufflerPro.Client;

namespace ShufflerPro.Bootstrapper.StartupTasks;

public class GeniusAccessStartupTask : IStartupTask
{
    private readonly AccessKeysContainer _accessKeysContainer;

    public GeniusAccessStartupTask(AccessKeysContainer accessKeysContainer)
    {
        _accessKeysContainer = accessKeysContainer;
    }

    public void Run()
    {
        Task.Run(async () => await BuildAccess());
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    private async Task BuildAccess()
    {
        try
        {
            using (var client = new HttpClient())
            {
                var requestBody =
                    new StringContent(
                        $"client_id={ClientId}&client_secret={ClientSecret}&grant_type=client_credentials",
                        Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await client.PostAsync(AccessTokenUrl, requestBody);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(jsonResponse);

                    var accessToken = json["access_token"]?.ToString();
                    if (accessToken == null)
                        return;

                    _accessKeysContainer.GeniusToken = accessToken;
                }
            }
        }
        catch (Exception)
        {
            _accessKeysContainer.GeniusToken = null;
        }
    }

// @formatter:off
    private const string ClientId = "t9Eb0UAPVbYDX-3qAkTVen7ODDvwW-O3yuux4d9iLycABvMtbCNmBlXWdNov1AYH";
    private const string ClientSecret = "HDRqfYXI3gLvkBy-EkzXtz8GsbU0PCaBh9BUkIvtgWSgjleyPWtrUkg4YrcJgtXycXKH3wWKzt_JnzUWyOqYvA";
    private const string AccessTokenUrl = "https://api.genius.com/oauth/token";
    // @formatter:on
}