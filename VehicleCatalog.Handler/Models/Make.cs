namespace VehicleCatalog.Handler.Models;

/// <summary>
/// A vehicle make (manufacturer), as exposed by the NHTSA vPIC "GetAllMakes" data set.
/// </summary>
/// <param name="Id">The NHTSA make identifier (<c>Make_ID</c>).</param>
/// <param name="Name">The make name (<c>Make_Name</c>).</param>
public sealed record Make(int Id, string Name);
