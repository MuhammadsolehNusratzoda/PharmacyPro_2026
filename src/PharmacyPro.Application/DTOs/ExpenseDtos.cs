using System.ComponentModel.DataAnnotations;

namespace PharmacyPro.Application.DTOs;

public record ExpenseCategoryDto(int Id, string Name);

public class CreateExpenseCategoryDto
{
    [Required, MaxLength(100)] public string Name { get; set; } = null!;
}

public record ExpenseDto(int Id, int CategoryId, string CategoryName, decimal Amount, DateTime ExpenseDate, string? Note);

public class CreateExpenseDto
{
    [Range(1, int.MaxValue)] public int CategoryId { get; set; }
    [Range(0.01, 1_000_000_000)] public decimal Amount { get; set; }
    [Required] public DateTime ExpenseDate { get; set; }
    [MaxLength(500)] public string? Note { get; set; }
}

public class UpdateExpenseDto : CreateExpenseDto { }