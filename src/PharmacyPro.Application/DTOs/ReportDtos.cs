namespace PharmacyPro.Application.DTOs;

public record CategoryExpenseDto(int CategoryId, string CategoryName, decimal Total);

public record FirmShareDto(int FirmId, string FirmName, decimal Total);

public record WeeklySaleDto(int WeekNumber, DateTime From, DateTime To, decimal Total);

public record MonthlyReportDto(
    int Year,
    int Month,
    decimal TotalRevenue,        // Даромади умумӣ
    decimal TotalPaidToFirms,    // Ба фирма дода шуд
    decimal TotalOperatingExpenses, // Свет, Аренда, Маош...
    decimal TotalCosts,          // Ҳароҷоти умумӣ = Firms + Operating
    decimal NetProfit,           // Фоидаи асосӣ
    bool IsLoss,
    List<CategoryExpenseDto> ExpensesByCategory,
    List<FirmShareDto> FirmShares);