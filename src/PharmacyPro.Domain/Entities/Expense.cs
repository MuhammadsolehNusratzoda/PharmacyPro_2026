using PharmacyPro.Domain.Common;

namespace PharmacyPro.Domain.Entities;

public class Expense : BaseEntity
{
    public int CategoryId { get; set; }
    public ExpenseCategory Category { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string? Note { get; set; }
    public string CreatedByUserId { get; set; } = null!;
}