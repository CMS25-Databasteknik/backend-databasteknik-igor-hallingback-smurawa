namespace Backend.Infrastructure.Entities
{
    public class CourseEventEntity
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public DateTime EventDate { get; set; }
        public decimal Price { get; set; }
        public int Seats { get; set; }
        public int CourseEventTypeId { get; set; }
        public CourseEntity Course { get; set; } = null!;
        public CourseEventTypeEntity CourseEventType { get; set; } = null!;
        public virtual ICollection<InPlaceEventLocationEntity> InPlaceEventLocations { get; set; } = [];
        public virtual ICollection<CourseEventInstructorEntity> CourseEventInstructors { get; set; } = [];
        public virtual ICollection<CourseRegistrationEntity> Registrations { get; set; } = [];
    }
}