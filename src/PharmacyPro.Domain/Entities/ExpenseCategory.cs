using PharmacyPro.Domain.Common;

namespace PharmacyPro.Domain.Entities;

public class ExpenseCategory : BaseEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}