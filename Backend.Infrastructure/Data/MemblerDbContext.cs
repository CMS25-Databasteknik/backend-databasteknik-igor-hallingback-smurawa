using Microsoft.EntityFrameworkCore;

namespace Backend.Infrastructure.Data
{
    public sealed class MemblerDbContext(DbContextOptions<MemblerDbContext> options) : DbContext(options)
    {
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
