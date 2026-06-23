using VehicleCatalog.Handler.Clients.Nhtsa;

namespace VehicleCatalog.Handler.Queries.Makes;

/// <summary>Handles <see cref="GetAllMakesQuery"/>.</summary>
public sealed class GetAllMakesQueryHandler : IRequestHandler<GetAllMakesQuery, IReadOnlyList<Make>>
{
    private readonly INhtsaVehicleClient client;

    /// <summary>Initializes a new instance of the <see cref="GetAllMakesQueryHandler"/> class.</summary>
    /// <param name="client">The NHTSA vehicle client.</param>
    public GetAllMakesQueryHandler(INhtsaVehicleClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        this.client = client;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Make>> Handle(GetAllMakesQuery request, CancellationToken cancellationToken) =>
        this.client.GetAllMakesAsync(cancellationToken);
}
