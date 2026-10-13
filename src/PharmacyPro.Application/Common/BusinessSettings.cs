namespace PharmacyPro.Application.Common;

public class BusinessSettings
{
    public const string SectionName = "Business";
    public string TimeZoneId { get; set; } = "Asia/Dushanbe";

    public TimeZoneInfo GetZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId); }
        catch (TimeZoneNotFoundException)
        {
            // Тоҷикистон DST надорад, пас UTC+5 доимист
            return TimeZoneInfo.CreateCustomTimeZone("TJT", TimeSpan.FromHours(5), "Tajikistan", "Tajikistan");
        }
    }
}