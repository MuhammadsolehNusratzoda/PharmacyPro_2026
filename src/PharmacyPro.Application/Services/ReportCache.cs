using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PharmacyPro.Application.Common;

namespace PharmacyPro.Application.Services;

public class ReportCache(IMemoryCache cache, IOptions<BusinessSettings> business)
{
    private readonly TimeZoneInfo _zone = business.Value.GetZone();

    public static string MonthlyKey(int y, int m) => $"report:monthly:{y}:{m}";
    public static string WeeklyKey(int y, int m) => $"report:weekly:{y}:{m}";

    public void Invalidate(DateTime utcDate)
    {
        var local = TimeZoneInfo.ConvertTimeFromUtc(utcDate.ToUtc(), _zone);
        cache.Remove(MonthlyKey(local.Year, local.Month));
        cache.Remove(WeeklyKey(local.Year, local.Month));
    }
}