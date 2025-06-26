public class BlocketTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsValid => !string.IsNullOrEmpty(AccessToken) && ExpiresAt > DateTime.UtcNow;
}