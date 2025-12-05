using System.Text.Json.Serialization;

namespace PlanPK.YouTrack
{
    public class YouTrackIssueDto
    {
        public string? Id { get; set; }

        [JsonPropertyName("idReadable")]
        public string? IdReadable { get; set; }

        public string? Summary { get; set; }

        public List<YouTrackIssueLinkDto> Links { get; set; } = new();
    }

    public class YouTrackIssueLinkDto
    {
        public YouTrackLinkTypeDto? LinkType { get; set; }

        public List<YouTrackLinkedIssueDto> Issues { get; set; } = new();
    }

    public class YouTrackLinkedIssueDto
    {
        [JsonPropertyName("idReadable")]
        public string? IdReadable { get; set; }
    }

    public class YouTrackLinkTypeDto
    {
        public string? Name { get; set; }

        public string? SourceToTarget { get; set; }

        public string? TargetToSource { get; set; }
    }
}
