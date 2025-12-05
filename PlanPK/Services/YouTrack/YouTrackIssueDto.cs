using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlanPK.Services.YouTrack;

public class YouTrackIssueDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("idReadable")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("summary")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("parent")]
    public YouTrackParentIssueDto? Parent { get; set; }

    [JsonPropertyName("customFields")]
    public List<YouTrackCustomFieldDto> CustomFields { get; set; } = new();
}

public class YouTrackParentIssueDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}

public class YouTrackCustomFieldDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public JsonElement? Value { get; set; }
        
}
