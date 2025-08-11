namespace TSG_Commex_Shared.DTOs;

public class CommunicationTypeStatusResponse
{
    public string StatusCode { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Phase { get; set; } = string.Empty;
    public string? TypeSpecificDescription { get; set; }
}
