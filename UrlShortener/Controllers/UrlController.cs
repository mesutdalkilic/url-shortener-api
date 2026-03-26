using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.DTOs;
using UrlShortener.Utils;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;


namespace UrlShortener.Controllers;

[ApiController]
[Route("")] // Root route -> short.ly/abc123 gibi çalışır
public class UrlController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache; // Redis cache için

    // Dependency Injection
    public UrlController(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache; // Redis cache için
    }

    // URL KISALTMA
    [HttpPost("shorten")]
    [EnableRateLimiting("fixed")] // Rate limiting uygulanır
    public IActionResult Shorten([FromBody] CreateShortUrlRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Url))
            return BadRequest("URL boş olamaz");

        if (!Uri.IsWellFormedUriString(request.Url, UriKind.Absolute))
            return BadRequest("Geçersiz URL");

        var shortUrl = new ShortUrl
        {
            OriginalUrl = request.Url
            // ShortCode sonra üretilecek
        };

        _context.ShortUrls.Add(shortUrl);
        _context.SaveChanges(); // ID burada oluşur

        // Base62 encode
        shortUrl.ShortCode = Base62Service.Encode(shortUrl.Id);

        _context.SaveChanges(); // tekrar kaydet

        var resultUrl = $"{Request.Scheme}://{Request.Host}/{shortUrl.ShortCode}";

        return Ok(resultUrl);
    }

    // REDIRECT
    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectToOriginal(string code)
    {
        var cacheKey = $"url:{code}"; // her URL için unique key

        // Cache kontrol
        var cachedData = await _cache.GetStringAsync(cacheKey);

        ShortUrl? url;

        if (cachedData != null)
        {
            // Cache HIT → hızlı
            url = JsonSerializer.Deserialize<ShortUrl>(cachedData);
        }
        else
        {
            // Cache MISS → DB'ye git
            url = _context.ShortUrls.FirstOrDefault(x => x.ShortCode == code);

            if (url != null)
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };

                // Cache'e yaz
                await _cache.SetStringAsync(
                    cacheKey,
                    JsonSerializer.Serialize(url),
                    options
                );
            }
        }

        if (url == null)
            return NotFound();

        // Click sayısı artır
        url.ClickCount++;
        _context.SaveChanges();

        return Redirect(url.OriginalUrl);

        if (url == null)
            return NotFound();


        url.ClickCount++; // tıklama artır
        _context.SaveChanges();

        return Redirect(url.OriginalUrl); // HTTP 302 redirect
    }



    // Kullanıcıya oluşturduğu linkleri listeleyebileceği bir endpoint eklenir ve click count ile basic analytics sağlanır.
    [HttpGet("links")]
    public IActionResult GetAll()
    {
        var urls = _context.ShortUrls
            .Select(x => new ShortUrlResponse
            {
                ShortUrl = $"{Request.Scheme}://{Request.Host}/{x.ShortCode}",
                OriginalUrl = x.OriginalUrl,
                ClickCount = x.ClickCount,
                CreatedAt = x.CreatedAt
            })
            .ToList();

        return Ok(urls);
    }


    [HttpGet("stats/{code}")]
    public IActionResult GetStats(string code)
    {
        var url = _context.ShortUrls.FirstOrDefault(x => x.ShortCode == code);

        if (url == null)
            return NotFound();

        var response = new ShortUrlResponse
        {
            ShortUrl = $"{Request.Scheme}://{Request.Host}/{url.ShortCode}",
            OriginalUrl = url.OriginalUrl,
            ClickCount = url.ClickCount,
            CreatedAt = url.CreatedAt
        };

        return Ok(response);
    }
}