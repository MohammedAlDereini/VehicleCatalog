namespace VehicleCatalog.Handler.Queries.VehicleTypes;

/// <summary>Retrieves the vehicle types available for a given make.</summary>
public sealed class GetVehicleTypesForMakeQuery : IRequest<IReadOnlyList<VehicleType>>
{
    /// <summary>Initializes a new instance of the <see cref="GetVehicleTypesForMakeQuery"/> class.</summary>
    /// <param name="makeId">The NHTSA make identifier.</param>
    public GetVehicleTypesForMakeQuery(int makeId) => this.MakeId = makeId;

    /// <summary>Gets the NHTSA make identifier.</summary>
    public int MakeId { get; }
}
