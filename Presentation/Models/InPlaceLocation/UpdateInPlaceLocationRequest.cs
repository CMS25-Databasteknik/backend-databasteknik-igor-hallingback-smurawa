namespace Backend.Presentation.API.Models.InPlaceLocation;

public sealed record UpdateInPlaceLocationRequest(
    int LocationId,
    int RoomNumber,
    int Seats
);
