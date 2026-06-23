using System.Text.Json.Serialization;

namespace VehicleCatalog.Handler.Nhtsa.Dtos;

/// <summary>
/// Envelope shared by every NHTSA vPIC JSON endpoint:
/// <c>{ Count, Message, SearchCriteria, Results }</c>.
/// </summary>
/// <typeparam name="TResult">The element type of the <c>Results</c> array.</typeparam>
internal sealed class NhtsaResponse<TResult>
{
    [JsonPropertyName("Count")]
    public int Count { get; set; }

    [JsonPropertyName("Message")]
    public string? Message { get; set; }

    [JsonPropertyName("Results")]
    public List<TResult>? Results { get; set; }
}
