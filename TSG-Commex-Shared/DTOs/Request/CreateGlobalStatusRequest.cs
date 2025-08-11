namespace TSG_Commex_Shared.DTOs.Request;

public class CreateGlobalStatusRequest
{
    public string StatusCode { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Phase { get; set; } // Creation, Production, Logistics, Terminal

}