using Microsoft.Extensions.DependencyInjection;
using PharmacyPro.Application.Interfaces;
using PharmacyPro.Application.Services;

namespace PharmacyPro.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<ReportCache>();

        services.AddScoped<IFirmService, FirmService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IExpenseCategoryService, ExpenseCategoryService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IFirmPaymentService, FirmPaymentService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}