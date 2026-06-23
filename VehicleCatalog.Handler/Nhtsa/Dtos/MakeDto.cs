using System.Text.Json.Serialization;

namespace VehicleCatalog.Handler.Nhtsa.Dtos;

/// <summary>An element of the "GetAllMakes" results array.</summary>
internal sealed class MakeDto
{
    [JsonPropertyName("Make_ID")]
    public int MakeId { get; set; }

    [JsonPropertyName("Make_Name")]
    public string? MakeName { get; set; }
}
