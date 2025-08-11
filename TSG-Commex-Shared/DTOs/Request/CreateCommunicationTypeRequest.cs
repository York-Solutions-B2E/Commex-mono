namespace TSG_Commex_Shared.DTOs.Request;

public class CreateCommunicationTypeRequest
{
    public string TypeCode { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<int> StatusIds { get; set; } = new();
}