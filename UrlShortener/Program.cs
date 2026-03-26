using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Caching.StackExchangeRedis;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// DB baðlantýsý
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=UrlShortenerDb;Trusted_Connection=True;")
);

// Controller
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


// Redis Cache Port: 6379, Host: localhost
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 1 kullanýcý  saniyede X istek, API’yi olaðan dýþý aþýrý istekten korumak için rate limiting uygularýz
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429; // Ýstek reddedildiðinde dönecek HTTP durum kodu (Too Many Requests)

    options.OnRejected = async (context, token) =>
    {
        // Response tipi JSON
        context.HttpContext.Response.ContentType = "application/json";

        // JSON body dön
        var response = new
        {
            error = "Too many requests"
        };

        var json = System.Text.Json.JsonSerializer.Serialize(response);

        await context.HttpContext.Response.WriteAsync(json, token);
    };


    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = 3; // 3 istek max istek sayýsý
        opt.Window = TimeSpan.FromSeconds(10); // 10 saniyede
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0; // bekleyen istek sayýsý
    });

    
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
