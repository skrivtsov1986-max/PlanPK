namespace PlanPK.Services.YouTrack;

public class YouTrackOptions
{
    public string BaseUrl { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public string Query { get; set; } = string.Empty;

    public string PlanHoursFieldName { get; set; } = "Plan";

    public string FaktHoursFieldName { get; set; } = "Fact";

    public string TermDateFieldName { get; set; } = "Due Date";

    public int PageSize { get; set; } = 200;
}
