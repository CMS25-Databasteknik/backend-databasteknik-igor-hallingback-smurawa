namespace Backend.Infrastructure.Entities
{
    public class StatusTypeEntity
    {
        public int Id { get; set; }
        public string StatusName { get; set; } = null!;
        public virtual ICollection<MemberEntity> Members { get; set; } = [];
    }
}
