namespace TSG_Commex_Shared.DTOs;

public class CommunicationStatusHistory
{
    public int Id { get; set; }
    public int CommunicationId { get; set; }
    public string StatusCode { get; set; } = string.Empty;
    public DateTime OccurredUtc { get; set; }
    public string? Notes { get; set; }
    public string? EventSource { get; set; }
    public string? UpdatedByUserName { get; set; }
}