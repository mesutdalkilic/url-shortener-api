using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.DTOs;

namespace UrlShortener.Controllers;

[ApiController]
[Route("")] // Root route -> short.ly/abc123 gibi çalışır
public class UrlController : ControllerBase
{
    private readonly AppDbContext _context;

    // Dependency Injection
    public UrlController(AppDbContext context)
    {
        _context = context;
    }

    // URL KISALTMA
    [HttpPost("shorten")]
    public IActionResult Shorten([FromBody] CreateShortUrlRequest request)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Url))
            return BadRequest("URL boş olamaz");

        if (!Uri.IsWellFormedUriString(request.Url, UriKind.Absolute))
            return BadRequest("Geçersiz URL");

        var shortCode = Guid.NewGuid().ToString().Substring(0, 6);

        var shortUrl = new ShortUrl
        {
            OriginalUrl = request.Url,
            ShortCode = shortCode
        };

        _context.ShortUrls.Add(shortUrl);
        _context.SaveChanges();

        var resultUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}";

        var response = new ShortUrlResponse
        {
            ShortUrl = resultUrl,
            OriginalUrl = shortUrl.OriginalUrl,
            ClickCount = shortUrl.ClickCount,
            CreatedAt = shortUrl.CreatedAt
        };

        return Ok(response);
    }

    // REDIRECT
    [HttpGet("{code}")]
    public IActionResult RedirectToOriginal(string code)
    {
        var url = _context.ShortUrls.FirstOrDefault(x => x.ShortCode == code);

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