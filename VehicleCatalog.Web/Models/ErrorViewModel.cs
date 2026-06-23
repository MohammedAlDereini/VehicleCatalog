namespace VehicleCatalog.Web.Models;

/// <summary>View model for the generic error page.</summary>
public sealed class ErrorViewModel
{
    /// <summary>The current request identifier, for correlating logs.</summary>
    public string? RequestId { get; init; }

    /// <summary>Whether a request identifier is available to display.</summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
}
