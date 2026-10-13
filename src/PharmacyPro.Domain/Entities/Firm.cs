using PharmacyPro.Domain.Common;

namespace PharmacyPro.Domain.Entities;

public class Firm : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<FirmPayment> Payments { get; set; } = new List<FirmPayment>();
}