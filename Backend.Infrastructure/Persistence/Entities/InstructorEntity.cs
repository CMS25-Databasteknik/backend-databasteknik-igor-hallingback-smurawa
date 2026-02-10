namespace Backend.Infrastructure.Persistence.Entities
{
    public class InstructorEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual ICollection<CourseEventEntity> CourseEvents { get; set; } = [];
    }
}