namespace TSG_Commex_Shared.DTOs.Request;

public class UpdateTypeStatusesRequest
{
    public List<int> StatusIds { get; set; } = new();
}