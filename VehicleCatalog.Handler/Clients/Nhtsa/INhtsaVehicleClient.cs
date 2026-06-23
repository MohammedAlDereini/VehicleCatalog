using VehicleCatalog.Handler.Models;

namespace VehicleCatalog.Handler.Clients.Nhtsa;

/// <summary>
/// Abstraction over the NHTSA vPIC vehicles API. Implemented in the
/// Infrastructure layer; the Application layer depends only on this contract.
/// </summary>
public interface INhtsaVehicleClient
{
    /// <summary>Gets every vehicle make known to the NHTSA vPIC data set.</summary>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>All makes, ordered by name.</returns>
    Task<IReadOnlyList<Make>> GetAllMakesAsync(CancellationToken cancellationToken);

    /// <summary>Gets the vehicle types available for a make.</summary>
    /// <param name="makeId">The NHTSA make identifier.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The vehicle types for the make, ordered by name.</returns>
    Task<IReadOnlyList<VehicleType>> GetVehicleTypesForMakeAsync(int makeId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the models for a make and model year, optionally constrained to a
    /// vehicle type.
    /// </summary>
    /// <param name="makeId">The NHTSA make identifier.</param>
    /// <param name="year">The model year.</param>
    /// <param name="vehicleType">Optional vehicle type name to filter by (e.g. "truck").</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The matching models, ordered by name.</returns>
    Task<IReadOnlyList<VehicleModel>> GetModelsAsync(int makeId, int year, string? vehicleType, CancellationToken cancellationToken);
}
