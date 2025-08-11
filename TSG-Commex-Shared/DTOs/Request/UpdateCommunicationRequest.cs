namespace TSG_Commex_Shared.DTOs.Request;

public class UpdateCommunicationRequest
{
    public string? Title { get; set; }
    public int? MemberId { get; set; }
    public string? SourceFileUrl { get; set; }
    public int? CurrentStatusId { get; set; }
    public int? UpdatedByUserId { get; set; }
}