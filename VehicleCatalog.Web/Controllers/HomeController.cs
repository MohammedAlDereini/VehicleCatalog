using System.Diagnostics;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VehicleCatalog.Handler.Queries.Makes;
using VehicleCatalog.Handler.Queries.Models;
using VehicleCatalog.Handler.Queries.VehicleTypes;
using VehicleCatalog.Web.Models;

namespace VehicleCatalog.Web.Controllers;

/// <summary>
/// Serves the vehicle catalog search page: pick a make and model year, then see
/// the vehicle types and models available for that selection.
/// </summary>
public sealed class HomeController : BaseController
{
    private const string UpstreamErrorMessage =
        "The vehicle data service (NHTSA) is currently unavailable. Please try again shortly.";

    /// <summary>Initializes a new instance of the <see cref="HomeController"/> class.</summary>
    /// <param name="mediator">The mediator used to dispatch queries.</param>
    public HomeController(IMediator mediator)
        : base(mediator)
    {
    }

    /// <summary>
    /// Renders the search form and, when a make and year are supplied, the
    /// matching vehicle types and models.
    /// </summary>
    /// <param name="makeId">The selected make identifier.</param>
    /// <param name="year">The selected model year.</param>
    /// <param name="vehicleType">An optional vehicle type name to filter models by.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The rendered search view.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Index(int? makeId, int? year, string? vehicleType, CancellationToken cancellationToken)
    {
        var years = BuildYears();

        try
        {
            var makes = await this.Mediator.Send(new GetAllMakesQuery(), cancellationToken);

            if (makeId is not > 0 || year is null)
            {
                return this.View(new VehicleSearchViewModel { Makes = makes, Years = years });
            }

            var types = await this.Mediator.Send(new GetVehicleTypesForMakeQuery(makeId.Value), cancellationToken);
            var models = await this.Mediator.Send(new GetModelsForMakeYearQuery(makeId.Value, year.Value, vehicleType), cancellationToken);

            return this.View(new VehicleSearchViewModel
            {
                Makes = makes,
                Years = years,
                SelectedMakeId = makeId,
                SelectedYear = year,
                SelectedVehicleType = string.IsNullOrWhiteSpace(vehicleType) ? null : vehicleType.Trim(),
                SelectedMakeName = makes.FirstOrDefault(m => m.Id == makeId.Value)?.Name,
                VehicleTypes = types,
                Models = models,
                HasSearched = true,
            });
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return this.View(new VehicleSearchViewModel
            {
                Years = years,
                SelectedMakeId = makeId,
                SelectedYear = year,
                ErrorMessage = UpstreamErrorMessage,
            });
        }
    }

    /// <summary>Renders the generic error page.</summary>
    /// <returns>The error view.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
}
