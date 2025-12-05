using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace PlanPK.YouTrack
{
    public class YouTrackClient
    {
        private readonly HttpClient _httpClient;
        private readonly YouTrackOptions _options;
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public YouTrackClient(HttpClient httpClient, IOptions<YouTrackOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
            {
                _httpClient.BaseAddress = new Uri(_options.BaseUrl);
            }
        }

        public async Task<List<YouTrackIssueDto>> GetIssuesAsync(CancellationToken cancellationToken = default)
        {
            ValidateConfiguration();

            var request = new HttpRequestMessage(HttpMethod.Get, BuildIssuesUrl());
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.Token);

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var issues = await JsonSerializer.DeserializeAsync<List<YouTrackIssueDto>>(contentStream, _serializerOptions, cancellationToken);

            return issues ?? new List<YouTrackIssueDto>();
        }

        private string BuildIssuesUrl()
        {
            var fields = Uri.EscapeDataString(_options.Fields);
            var query = Uri.EscapeDataString(_options.Query);
            return $"api/issues?fields={fields}&query={query}";
        }

        private void ValidateConfiguration()
        {
            if (string.IsNullOrWhiteSpace(_options.BaseUrl))
            {
                throw new InvalidOperationException("YouTrack base url is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_options.Token))
            {
                throw new InvalidOperationException("YouTrack token is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_options.Query))
            {
                throw new InvalidOperationException("YouTrack query is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_options.Fields))
            {
                throw new InvalidOperationException("YouTrack fields are not configured.");
            }
        }
    }
}
