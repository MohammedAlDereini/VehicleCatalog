using System.Globalization;
using System.Net.Http.Json;
using VehicleCatalog.Handler.Models;
using VehicleCatalog.Handler.Nhtsa.Dtos;

namespace VehicleCatalog.Handler.Clients.Nhtsa;

/// <summary>
/// <see cref="INhtsaVehicleClient"/> implementation backed by the NHTSA vPIC HTTP API.
/// </summary>
public sealed class NhtsaVehicleClient : INhtsaVehicleClient
{
    private readonly HttpClient httpClient;

    /// <summary>Initializes a new instance of the <see cref="NhtsaVehicleClient"/> class.</summary>
    /// <param name="httpClient">The typed HTTP client (base address configured at registration).</param>
    public NhtsaVehicleClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        this.httpClient = httpClient;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Make>> GetAllMakesAsync(CancellationToken cancellationToken)
    {
        var dtos = await this.GetResultsAsync<MakeDto>("vehicles/getallmakes?format=json", cancellationToken);
        return dtos
            .Where(d => !string.IsNullOrWhiteSpace(d.MakeName))
            .Select(d => new Make(d.MakeId, d.MakeName!.Trim()))
            .OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<VehicleType>> GetVehicleTypesForMakeAsync(int makeId, CancellationToken cancellationToken)
    {
        var url = string.Create(CultureInfo.InvariantCulture, $"vehicles/GetVehicleTypesForMakeId/{makeId}?format=json");
        var dtos = await this.GetResultsAsync<VehicleTypeDto>(url, cancellationToken);
        return dtos
            .Where(d => !string.IsNullOrWhiteSpace(d.VehicleTypeName))
            .Select(d => new VehicleType(d.VehicleTypeId, d.VehicleTypeName!.Trim()))
            .OrderBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<VehicleModel>> GetModelsAsync(int makeId, int year, string? vehicleType, CancellationToken cancellationToken)
    {
        var url = string.IsNullOrWhiteSpace(vehicleType)
            ? string.Create(CultureInfo.InvariantCulture, $"vehicles/GetModelsForMakeIdYear/makeId/{makeId}/modelyear/{year}?format=json")
            : string.Create(CultureInfo.InvariantCulture, $"vehicles/GetModelsForMakeIdYear/makeId/{makeId}/modelyear/{year}/vehicletype/{Uri.EscapeDataString(vehicleType.Trim())}?format=json");

        var dtos = await this.GetResultsAsync<ModelDto>(url, cancellationToken);
        return dtos
            .Where(d => !string.IsNullOrWhiteSpace(d.ModelName))
            .Select(d => new VehicleModel(d.ModelId, d.ModelName!.Trim()))
            .OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task<List<TDto>> GetResultsAsync<TDto>(string relativeUrl, CancellationToken cancellationToken)
    {
        var payload = await this.httpClient.GetFromJsonAsync<NhtsaResponse<TDto>>(relativeUrl, cancellationToken);
        return payload?.Results ?? new List<TDto>();
    }
}
