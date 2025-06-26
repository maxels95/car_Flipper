// Models/BlocketTokenResponse.cs
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;


public class BlocketAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private BlocketTokenResponse _cachedToken = new();
    private readonly string _endpoint = "https://www.blocket.se/api/adout-api-route/refresh-token-and-validate-session";

    public BlocketAuthService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string?> GetTokenAsync(bool force = false)
    {
        if (!force && _cachedToken.IsValid)
            return _cachedToken.AccessToken;

        var clientSecret = _config["Blocket:ClientSecret"];
        if (string.IsNullOrWhiteSpace(clientSecret))
            throw new InvalidOperationException("ClientSecret saknas i konfigurationen.");

        var payload = new { clientSecret };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, _endpoint)
        {
            Content = content
        };
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Kunde inte hämta token: {response.StatusCode}");

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var accessToken = root.GetProperty("bearerToken").GetString();

        var random = new Random();
        int jitterSeconds = random.Next(-60, 300); // mellan -1 min till +5 min

        _cachedToken = new BlocketTokenResponse
        {
            AccessToken = accessToken ?? string.Empty,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30).AddSeconds(jitterSeconds)
        };


        return _cachedToken.AccessToken;
    }
}
