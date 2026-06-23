namespace VehicleCatalog.Handler.Models;

/// <summary>
/// A vehicle model for a given make and model year, as exposed by the NHTSA
/// vPIC "GetModelsForMakeIdYear" data set.
/// </summary>
/// <param name="Id">The model identifier (<c>Model_ID</c>).</param>
/// <param name="Name">The model name (<c>Model_Name</c>).</param>
public sealed record VehicleModel(int Id, string Name);
