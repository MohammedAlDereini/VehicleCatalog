namespace VehicleCatalog.Handler.Queries.Models;

/// <summary>
/// Retrieves the models for a make and model year, optionally filtered by a
/// vehicle type.
/// </summary>
public sealed class GetModelsForMakeYearQuery : IRequest<IReadOnlyList<VehicleModel>>
{
    /// <summary>Initializes a new instance of the <see cref="GetModelsForMakeYearQuery"/> class.</summary>
    /// <param name="makeId">The NHTSA make identifier.</param>
    /// <param name="year">The model year.</param>
    /// <param name="vehicleType">Optional vehicle type name to filter by.</param>
    public GetModelsForMakeYearQuery(int makeId, int year, string? vehicleType = null)
    {
        this.MakeId = makeId;
        this.Year = year;
        this.VehicleType = string.IsNullOrWhiteSpace(vehicleType) ? null : vehicleType.Trim();
    }

    /// <summary>Gets the NHTSA make identifier.</summary>
    public int MakeId { get; }

    /// <summary>Gets the model year.</summary>
    public int Year { get; }

    /// <summary>Gets the optional vehicle type name filter.</summary>
    public string? VehicleType { get; }
}
