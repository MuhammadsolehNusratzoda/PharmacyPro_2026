using System.ComponentModel.DataAnnotations;
using PharmacyPro.Domain.Enums;

namespace PharmacyPro.Application.DTOs;

public record SaleDto(int Id, decimal Amount, DateTime SaleDate, PaymentMethod Method, string? Note, DateTime CreatedAt);

public class CreateSaleDto
{
    [Range(0.01, 1_000_000_000)] public decimal Amount { get; set; }
    [Required] public DateTime SaleDate { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    [MaxLength(500)] public string? Note { get; set; }
}

public class UpdateSaleDto : CreateSaleDto { }