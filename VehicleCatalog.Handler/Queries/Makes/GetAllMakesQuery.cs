namespace VehicleCatalog.Handler.Queries.Makes;

/// <summary>Retrieves every vehicle make from the NHTSA vPIC data set.</summary>
public sealed class GetAllMakesQuery : IRequest<IReadOnlyList<Make>>
{
}
