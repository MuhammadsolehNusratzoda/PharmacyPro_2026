namespace PharmacyPro.Application.Common;

public static class DateHelper
{
    // Npgsql танҳо UTC қабул мекунад
    public static DateTime ToUtc(this DateTime d) =>
        d.Kind == DateTimeKind.Utc ? d
        : d.Kind == DateTimeKind.Local ? d.ToUniversalTime()
        : DateTime.SpecifyKind(d, DateTimeKind.Utc);

    public static DateTime EndExclusive(this DateTime d) => d.ToUtc().Date.AddDays(1);
}