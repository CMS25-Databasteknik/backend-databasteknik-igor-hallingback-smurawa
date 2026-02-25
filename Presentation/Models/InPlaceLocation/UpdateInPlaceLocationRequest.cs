using System.ComponentModel.DataAnnotations;

namespace Backend.Presentation.API.Models.InPlaceLocation;

public sealed record UpdateInPlaceLocationRequest
{
    [Range(1, int.MaxValue)]
    public required int LocationId { get; init; }

    [Range(1, int.MaxValue)]
    public required int RoomNumber { get; init; }

    [Range(1, int.MaxValue)]
    public required int Seats { get; init; }
}
