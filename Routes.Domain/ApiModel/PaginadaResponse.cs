using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Routes.Domain.ApiModel;

public class PaginadaResponse<T>
{
    [JsonPropertyName("hasMore")]
    public bool HasMore { get; set; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("data")]
    public List<T> Data { get; set; }
}