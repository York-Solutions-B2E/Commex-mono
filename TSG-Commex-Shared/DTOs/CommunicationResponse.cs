namespace TSG_Commex_Shared.DTOs;

public class CommunicationResponse
{
    public int Id { get; set; }  // Keep the ID!
    
    // Type info (both ID and code for flexibility)
    public int CommunicationTypeId { get; set; }
    public string TypeCode { get; set; } = string.Empty;
    
    // Status info (both ID and code)
    public int CurrentStatusId { get; set; }
    public string CurrentStatus { get; set; } = string.Empty;
    
    // Member info
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    
    // Communication details
    public string Subject { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string RecipientInfo { get; set; } = string.Empty;
    
    // Metadata
    public DateTime CreatedUtc { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
}