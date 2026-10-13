using System.ComponentModel.DataAnnotations;

namespace PharmacyPro.Application.DTOs;

public record FirmDto(int Id, string Name, string? Phone, string? Address, bool IsActive, DateTime CreatedAt);

public class CreateFirmDto
{
    [Required, MaxLength(150)] public string Name { get; set; } = null!;
    [MaxLength(30)] public string? Phone { get; set; }
    [MaxLength(250)] public string? Address { get; set; }
}

public class UpdateFirmDto : CreateFirmDto
{
    public bool IsActive { get; set; } = true;
}