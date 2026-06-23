using VehicleCatalog.Handler.Clients.Nhtsa;

namespace VehicleCatalog.Handler.Queries.Models;

/// <summary>Handles <see cref="GetModelsForMakeYearQuery"/>.</summary>
public sealed class GetModelsForMakeYearQueryHandler
    : IRequestHandler<GetModelsForMakeYearQuery, IReadOnlyList<VehicleModel>>
{
    private readonly INhtsaVehicleClient client;

    /// <summary>Initializes a new instance of the <see cref="GetModelsForMakeYearQueryHandler"/> class.</summary>
    /// <param name="client">The NHTSA vehicle client.</param>
    public GetModelsForMakeYearQueryHandler(INhtsaVehicleClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        this.client = client;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<VehicleModel>> Handle(GetModelsForMakeYearQuery request, CancellationToken cancellationToken) =>
        this.client.GetModelsAsync(request.MakeId, request.Year, request.VehicleType, cancellationToken);
}
