using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data
{
    public sealed class MemblerDbContext(DbContextOptions<MemblerDbContext> options) : DbContext(options)
    {
        public DbSet<MemberEntity> Members => Set<MemberEntity>();
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemberEntity>(entity =>
            {
                entity.ToTable("Members");

                entity.HasKey(e => e.Id).HasName("PK_Members_Id");

                entity.Property(entity => entity.Id)
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("(NEWSEQUENTIALID())", "DF_Members_Id");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
