namespace WareManagement.DTO.PartnerDTO;

public class PartnerResponseDto
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxCode { get; set; }
}
