using Microsoft.EntityFrameworkCore;
using PersonelApi.Models;

namespace PersonelApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Personel> Personeller { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Personel>(entity =>
            {
                entity.ToTable("Personeller");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Ad).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Soyad).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CvFileName).IsRequired(false).HasMaxLength(255);
                entity.Property(e => e.CvOriginalFileName).IsRequired(false).HasMaxLength(255);
            });
        }
    }
}
