using Microsoft.EntityFrameworkCore;
using SciMaterials.DAL.Entities;

namespace SciMaterials.DAL.Contexts
{
    public class MaterialsContext : DbContext
    {
        public DbSet<FileModel> Files { get; set; } = default!;

        public MaterialsContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
