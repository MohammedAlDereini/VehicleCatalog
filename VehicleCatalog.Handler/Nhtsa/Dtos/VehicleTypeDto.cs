using System.Text.Json.Serialization;

namespace VehicleCatalog.Handler.Nhtsa.Dtos;

/// <summary>An element of the "GetVehicleTypesForMakeId" results array.</summary>
internal sealed class VehicleTypeDto
{
    [JsonPropertyName("MakeId")]
    public int MakeId { get; set; }

    [JsonPropertyName("MakeName")]
    public string? MakeName { get; set; }

    [JsonPropertyName("VehicleTypeId")]
    public int VehicleTypeId { get; set; }

    [JsonPropertyName("VehicleTypeName")]
    public string? VehicleTypeName { get; set; }
}
