using VehicleCatalog.Handler.Clients.Nhtsa;

namespace VehicleCatalog.Handler.Queries.VehicleTypes;

/// <summary>Handles <see cref="GetVehicleTypesForMakeQuery"/>.</summary>
public sealed class GetVehicleTypesForMakeQueryHandler
    : IRequestHandler<GetVehicleTypesForMakeQuery, IReadOnlyList<VehicleType>>
{
    private readonly INhtsaVehicleClient client;

    /// <summary>Initializes a new instance of the <see cref="GetVehicleTypesForMakeQueryHandler"/> class.</summary>
    /// <param name="client">The NHTSA vehicle client.</param>
    public GetVehicleTypesForMakeQueryHandler(INhtsaVehicleClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        this.client = client;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<VehicleType>> Handle(GetVehicleTypesForMakeQuery request, CancellationToken cancellationToken) =>
        this.client.GetVehicleTypesForMakeAsync(request.MakeId, cancellationToken);
}
