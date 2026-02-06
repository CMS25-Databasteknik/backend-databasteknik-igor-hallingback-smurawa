namespace Backend.Infrastructure.Entities
{
    public class RoleEntity
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public virtual ICollection<MemberEntity> Members { get; set; } = [];
    }
}
