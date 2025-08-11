namespace TSG_Commex_Shared.DTOs.Response;

public class GlobalStatusResponse
{
    public int Id { get; set; }
    public string StatusCode { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Phase { get; set; }
}