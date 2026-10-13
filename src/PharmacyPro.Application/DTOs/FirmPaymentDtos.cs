using System.ComponentModel.DataAnnotations;
using PharmacyPro.Domain.Enums;

namespace PharmacyPro.Application.DTOs;

public record FirmPaymentDto(int Id, int FirmId, string FirmName, decimal Amount, DateTime PaymentDate, PaymentMethod Method, string? Note);

public class CreateFirmPaymentDto
{
    [Range(1, int.MaxValue)] public int FirmId { get; set; }
    [Range(0.01, 1_000_000_000)] public decimal Amount { get; set; }
    [Required] public DateTime PaymentDate { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    [MaxLength(500)] public string? Note { get; set; }
}

public class UpdateFirmPaymentDto : CreateFirmPaymentDto { }