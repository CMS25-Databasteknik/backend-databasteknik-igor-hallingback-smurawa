namespace Backend.Infrastructure.Entities
{
    public class CourseEventTypeEntity
    {
        public int Id { get; set; }
        public string TypeName { get; set; } = null!;
        public virtual ICollection<CourseEventEntity> CourseEvents { get; set; } = [];
    }
}