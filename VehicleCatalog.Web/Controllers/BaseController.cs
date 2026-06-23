using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace VehicleCatalog.Web.Controllers;

/// <summary>
/// Base MVC controller exposing the mediator and shared helpers to every
/// controller in the application.
/// </summary>
public abstract class BaseController : Controller
{
    /// <summary>Earliest model year offered in the year selector.</summary>
    protected const int MinYear = 1995;

    /// <summary>Initializes a new instance of the <see cref="BaseController"/> class.</summary>
    /// <param name="mediator">The mediator used to dispatch commands and queries.</param>
    protected BaseController(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        this.Mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>Gets the mediator used to dispatch commands and queries.</summary>
    protected IMediator Mediator { get; }

    /// <summary>Builds the list of selectable model years, newest first.</summary>
    /// <returns>Years from next model year down to <see cref="MinYear"/>.</returns>
    protected List<int> BuildYears()
    {
        var maxYear = DateTime.UtcNow.Year + 1;
        var years = new List<int>();
        for (var y = maxYear; y >= MinYear; y--)
        {
            years.Add(y);
        }

        return years;
    }
}
