using PharmacyPro.Domain.Common;
using PharmacyPro.Domain.Enums;

namespace PharmacyPro.Domain.Entities;

public class FirmPayment : BaseEntity
{
    public int FirmId { get; set; }
    public Firm Firm { get; set; } = null!;

    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    public string? Note { get; set; }
}