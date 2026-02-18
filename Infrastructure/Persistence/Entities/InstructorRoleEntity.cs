namespace Backend.Infrastructure.Persistence.Entities;

public class InstructorRoleEntity
{
    public int Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public byte[] Concurrency { get; set; } = null!;

    public virtual ICollection<InstructorEntity> Instructors { get; set; } = [];
}
