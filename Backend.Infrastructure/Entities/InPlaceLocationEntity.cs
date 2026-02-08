namespace Backend.Infrastructure.Entities
{
    public class InPlaceLocationEntity
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public int RoomNumber { get; set; }
        public int Seats { get; set; }
        public LocationEntity Location { get; set; } = null!;
        public virtual ICollection<InPlaceEventLocationEntity> InPlaceEventLocations { get; set; } = [];
    }
}