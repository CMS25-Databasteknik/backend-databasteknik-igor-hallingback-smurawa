namespace Backend.Domain.Modules.InPlaceLocations.Models;

public sealed class InPlaceLocation
{
    public int Id { get; }
    public int LocationId { get; }
    public int RoomNumber { get; }
    public int Seats { get; }

    public InPlaceLocation(int id, int locationId, int roomNumber, int seats)
    {
        if (id < 0)
            throw new ArgumentException("ID must be greater than or equal to zero.", nameof(id));

        if (locationId <= 0)
            throw new ArgumentException("Location ID must be greater than zero.", nameof(locationId));

        if (roomNumber <= 0)
            throw new ArgumentException("Room number must be greater than zero.", nameof(roomNumber));

        if (seats <= 0)
            throw new ArgumentException("Seats must be greater than zero.", nameof(seats));

        Id = id;
        LocationId = locationId;
        RoomNumber = roomNumber;
        Seats = seats;
    }
}
