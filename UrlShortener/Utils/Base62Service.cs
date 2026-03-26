namespace UrlShortener.Utils;

public static class Base62Service
{
    // Base62 karakter seti
    private const string Characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Encode(int value)
    {
        // Güvenlik: 0 kontrolü
        if (value == 0)
            return "0";

        var result = string.Empty;

        // Base62 encoding algoritması
        while (value > 0)
        {
            result = Characters[value % 62] + result;
            value /= 62;
        }

        return result;
    }
}