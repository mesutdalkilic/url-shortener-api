namespace UrlShortener.Models
{
    public class ShortUrl
    {
        public int Id { get; set; } // Primary key (otomatik artar)

        public string OriginalUrl { get; set; } = string.Empty; // Kullanıcının gönderdiği uzun link Ayrıca C# nullable reference types nedeniyle default değer atadım.

        public string ShortCode { get; set; } = string.Empty; // Kısa kod (örn: Ab3Xy) Ayrıca Null warning çözümü

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma zamanı

        public int ClickCount { get; set; } = 0; // Tıklanma sayısı
    }
}
