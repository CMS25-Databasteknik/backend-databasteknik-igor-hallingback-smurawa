namespace Backend.Infrastructure.Entities
{
    public class MemberEntity
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public byte[] Concurrency { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime ModifiedAtUtc { get; set; }
        public virtual ICollection<RoleEntity> Roles { get; set; } = [];
    }
}
