namespace Application.Commons.Tools;
public static class Generator
{
    private static readonly Random Random = new Random();
    private const string? Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public static string GenerateId()
    {
        var id = new char[7];
        for (int i = 0; i < 7; i++)
        {
            id[i] = Chars[Random.Next(Chars.Length)];
        }
        return new string(id);
    }
    public static long GeneratePaymentCode()
    {
        var random = new Random();
        var minValue = 1_000_000_000;
        var maxValue = 9_999_999_999;
        var result = (long)(random.NextDouble() * (maxValue - minValue) + minValue);
        return result;
    }
}