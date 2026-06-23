using VehicleCatalog.Handler.Models;

namespace VehicleCatalog.Web.Models;

/// <summary>
/// View model backing the vehicle catalog search page: the selectable criteria
/// (make + year, optional vehicle type) and the results returned for them.
/// </summary>
public sealed class VehicleSearchViewModel
{
    /// <summary>All selectable makes, ordered by name.</summary>
    public IReadOnlyList<Make> Makes { get; init; } = [];

    /// <summary>Selectable model years, newest first.</summary>
    public IReadOnlyList<int> Years { get; init; } = [];

    /// <summary>The selected make identifier, if any.</summary>
    public int? SelectedMakeId { get; init; }

    /// <summary>The selected model year, if any.</summary>
    public int? SelectedYear { get; init; }

    /// <summary>The selected vehicle type name filter, if any.</summary>
    public string? SelectedVehicleType { get; init; }

    /// <summary>The display name of the selected make, if resolved.</summary>
    public string? SelectedMakeName { get; init; }

    /// <summary>Vehicle types available for the selected make.</summary>
    public IReadOnlyList<VehicleType> VehicleTypes { get; init; } = [];

    /// <summary>Models matching the selected criteria.</summary>
    public IReadOnlyList<VehicleModel> Models { get; init; } = [];

    /// <summary>Whether a search has been performed (controls results rendering).</summary>
    public bool HasSearched { get; init; }

    /// <summary>A user-facing error message (e.g. upstream unavailable), if any.</summary>
    public string? ErrorMessage { get; init; }
}
