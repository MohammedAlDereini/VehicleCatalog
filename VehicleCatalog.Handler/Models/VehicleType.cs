namespace VehicleCatalog.Handler.Models;

/// <summary>
/// A vehicle type available for a given make, as exposed by the NHTSA vPIC
/// "GetVehicleTypesForMakeId" data set.
/// </summary>
/// <param name="Id">The vehicle type identifier (<c>VehicleTypeId</c>).</param>
/// <param name="Name">The vehicle type name (<c>VehicleTypeName</c>), e.g. "Passenger Car".</param>
public sealed record VehicleType(int Id, string Name);
