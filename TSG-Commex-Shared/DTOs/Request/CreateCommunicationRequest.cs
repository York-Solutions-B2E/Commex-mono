namespace TSG_Commex_Shared.DTOs.Request;

public class CreateCommunicationRequest
{
    public int CommunicationTypeId { get; set; }
    public int MemberId { get; set; }  // Required - every communication needs a member
    public string Title { get; set; } = string.Empty;
    public string? SourceFileUrl { get; set; }
    public int? InitialStatusId { get; set; } // Optional - defaults to "ReadyForRelease" if not provided
    public int? CreatedByUserId { get; set; }
}