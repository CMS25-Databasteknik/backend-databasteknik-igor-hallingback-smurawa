namespace Backend.Infrastructure.Entities
{
    public class CourseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Length { get; set; }
        public virtual ICollection<CourseEventEntity> CourseEvents { get; set; } = [];
    }
}
