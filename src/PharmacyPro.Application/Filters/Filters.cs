using PharmacyPro.Domain.Enums;

namespace PharmacyPro.Application.Filters;

public class FirmFilter : PaginationFilter
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
}

public class SaleFilter : PaginationFilter
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public PaymentMethod? Method { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
}

public class ExpenseFilter : PaginationFilter
{
    public int? CategoryId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}

public class FirmPaymentFilter : PaginationFilter
{
    public int? FirmId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}