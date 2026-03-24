using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UrlShortener.Models;

namespace UrlShortener.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ShortUrl> ShortUrls { get; set; } // Bu tabloyu DB’de oluşturur
}