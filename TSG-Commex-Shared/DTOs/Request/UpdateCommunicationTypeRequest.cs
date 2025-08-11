namespace TSG_Commex_Shared.DTOs.Request;

public class UpdateCommunicationTypeRequest
{
    public string? TypeCode { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
    public List<int>? StatusIds { get; set; }
}