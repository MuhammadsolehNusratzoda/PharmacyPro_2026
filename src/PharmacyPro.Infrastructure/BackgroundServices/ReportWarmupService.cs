using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PharmacyPro.Application.Interfaces;

namespace PharmacyPro.Infrastructure.BackgroundServices;

// Cache-и ҳисоботи моҳи ҷориро ҳамеша «гарм» нигоҳ медорад (TTL = 10 дақиқа)
public class ReportWarmupService(
    IServiceScopeFactory scopeFactory,
    ILogger<ReportWarmupService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(9);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(Interval);
        try
        {
            do { await RunAsync(); }
            while (await timer.WaitForNextTickAsync(ct));
        }
        catch (OperationCanceledException) { }
    }

    private async Task RunAsync()
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var reports = scope.ServiceProvider.GetRequiredService<IReportService>();
            var now = DateTime.UtcNow;

            await reports.GetMonthlyReportAsync(now.Year, now.Month);
            await reports.GetWeeklySalesAsync(now.Year, now.Month);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Report warmup failed");
        }
    }
}