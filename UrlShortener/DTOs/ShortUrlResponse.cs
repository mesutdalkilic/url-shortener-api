namespace UrlShortener.DTOs;

public class ShortUrlResponse
{
    public string ShortUrl { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public int ClickCount { get; set; }
    public DateTime CreatedAt { get; set; }
}