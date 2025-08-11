namespace TSG_Commex_Shared.DTOs.Response;

public class CommunicationTypeResponse
{
    public int Id { get; set; }
    public string TypeCode { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<int> AssignedStatusIds { get; set; } = new();
}