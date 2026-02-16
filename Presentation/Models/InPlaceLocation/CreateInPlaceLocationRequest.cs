namespace Backend.Presentation.API.Models.InPlaceLocation;

public sealed record CreateInPlaceLocationRequest(
    int LocationId,
    int RoomNumber,
    int Seats
);
