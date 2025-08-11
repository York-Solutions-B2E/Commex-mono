namespace TSG_Commex_Shared.DTOs.Request;

public class GetCommunicationRequest
{
    public int? StatusId { get; set; }
    public int? TypeId { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}