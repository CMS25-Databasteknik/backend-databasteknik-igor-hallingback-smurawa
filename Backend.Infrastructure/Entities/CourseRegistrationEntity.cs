namespace Backend.Infrastructure.Entities
{
    public class CourseRegistrationEntity
    {
        public Guid Id { get; set; }
        public Guid ParticipantId { get; set; }
        public Guid CourseEventId { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsPaid { get; set; }
        public byte[] Concurrency { get; set; } = null!;
        public DateTime ModifiedAtUtc { get; set; }
        public ParticipantEntity Participant { get; set; } = null!;
        public CourseEventEntity CourseEvent { get; set; } = null!;
    }
}