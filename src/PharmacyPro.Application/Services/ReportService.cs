using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PharmacyPro.Application.Common;
using PharmacyPro.Application.DTOs;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Application.Responses;
using PharmacyPro.Domain.Entities;

namespace PharmacyPro.Application.Services;

public class ReportService(
    IRepository<Sale> saleRepo,
    IRepository<Expense> expenseRepo,
    IRepository<FirmPayment> paymentRepo,
    IMemoryCache cache,
    IOptions<BusinessSettings> business) : IReportService
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);
    private readonly TimeZoneInfo _zone = business.Value.GetZone();

    private static bool IsValid(int year, int month) =>
        year is >= 2000 and <= 2100 && month is >= 1 and <= 12;

    // 1-уми моҳ 00:00 бо вақти Тоҷикистон → UTC
    private (DateTime from, DateTime to) Range(int year, int month)
    {
        var localStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Unspecified);
        return (TimeZoneInfo.ConvertTimeToUtc(localStart, _zone),
                TimeZoneInfo.ConvertTimeToUtc(localStart.AddMonths(1), _zone));
    }

    public async Task<Response<MonthlyReportDto>> GetMonthlyReportAsync(int year, int month)
    {
        if (!IsValid(year, month)) return Response<MonthlyReportDto>.Fail("Сол ё моҳ нодуруст аст");

        var report = await cache.GetOrCreateAsync(ReportCache.MonthlyKey(year, month), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheTtl;
            var (from, to) = Range(year, month);

            var revenue = await saleRepo.Query()
                .Where(s => s.SaleDate >= from && s.SaleDate < to)
                .Select(s => s.Amount).SumAsync();

            // Аввал аз DB бо anonymous type мегирем, баъд дар хотира ба DTO мепайвандем
            var categoryRows = await expenseRepo.Query()
                .Where(e => e.ExpenseDate >= from && e.ExpenseDate < to)
                .GroupBy(e => new { e.CategoryId, e.Category.Name })
                .Select(g => new { g.Key.CategoryId, g.Key.Name, Total = g.Sum(x => x.Amount) })
                .ToListAsync();

            var byCategory = categoryRows
                .Select(r => new CategoryExpenseDto(r.CategoryId, r.Name, r.Total))
                .OrderByDescending(x => x.Total)
                .ToList();

            var firmRows = await paymentRepo.Query()
                .Where(p => p.PaymentDate >= from && p.PaymentDate < to)
                .GroupBy(p => new { p.FirmId, p.Firm.Name })
                .Select(g => new { g.Key.FirmId, g.Key.Name, Total = g.Sum(x => x.Amount) })
                .ToListAsync();

            var firmShares = firmRows
                .Select(r => new FirmShareDto(r.FirmId, r.Name, r.Total))
                .OrderByDescending(x => x.Total)
                .ToList();

            var operating = byCategory.Sum(x => x.Total);
            var paidToFirms = firmShares.Sum(x => x.Total);
            var totalCosts = operating + paidToFirms;
            var net = revenue - totalCosts;

            return new MonthlyReportDto(year, month, revenue, paidToFirms, operating,
                totalCosts, net, net < 0, byCategory, firmShares);
        });

        return Response<MonthlyReportDto>.Success(report!);
    }

    public async Task<Response<List<WeeklySaleDto>>> GetWeeklySalesAsync(int year, int month)
    {
        if (!IsValid(year, month)) return Response<List<WeeklySaleDto>>.Fail("Сол ё моҳ нодуруст аст");

        var weeks = await cache.GetOrCreateAsync(ReportCache.WeeklyKey(year, month), async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheTtl;
            var (from, to) = Range(year, month);

            var raw = await saleRepo.Query()
                .Where(s => s.SaleDate >= from && s.SaleDate < to)
                .Select(s => new { s.SaleDate, s.Amount })
                .ToListAsync();

            // рӯз бо вақти маҳаллӣ (Тоҷикистон)
            var sales = raw
                .Select(s => new { Day = TimeZoneInfo.ConvertTimeFromUtc(s.SaleDate, _zone).Day, s.Amount })
                .ToList();

            var daysInMonth = DateTime.DaysInMonth(year, month);
            var result = new List<WeeklySaleDto>();

            // Ҳафта 1: 1-7, 2: 8-14, 3: 15-21, 4: 22-28, 5: 29-охир
            for (int w = 1; w <= 5; w++)
            {
                var startDay = (w - 1) * 7 + 1;
                if (startDay > daysInMonth) break;
                var endDay = w == 5 ? daysInMonth : Math.Min(startDay + 6, daysInMonth);

                var total = sales.Where(s => s.Day >= startDay && s.Day <= endDay).Sum(s => s.Amount);

                result.Add(new WeeklySaleDto(
                    w,
                    new DateTime(year, month, startDay, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(year, month, endDay, 0, 0, 0, DateTimeKind.Utc),
                    total));
            }
            return result;
        });

        return Response<List<WeeklySaleDto>>.Success(weeks!);
    }
}