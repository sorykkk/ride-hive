using Microsoft.EntityFrameworkCore;
using RideHiveApi.Models;

namespace RideHiveApi.Data 
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<CarItem> CarItems { get; set; }
        public DbSet<CarImageData> CarImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure CarItem entity
            modelBuilder.Entity<CarItem>(entity =>
                {
                    entity.HasKey(e => e.CarId);
                    entity.Property(e => e.CarId).ValueGeneratedOnAdd();

                    // Configure indexes for better performance
                    entity.HasIndex(e => e.OwnerId);
                    entity.HasIndex(e => e.VinNumber).IsUnique();
                    entity.HasIndex(e => new { e.Brand, e.Model });
                }
            );

            // Configure CarImageData entity
            modelBuilder.Entity<CarImageData>(entity =>
                {
                    entity.HasKey(e => e.CarImageId);
                    entity.Property(e => e.CarImageId).ValueGeneratedOnAdd();

                    // Configure relationship with CarItem
                    entity.HasOne<CarItem>()
                        .WithMany(c => c.CarImages)
                        .HasForeignKey(ci => ci.CarId)
                        .OnDelete(DeleteBehavior.Cascade);

                    // Configure indexes
                    entity.HasIndex(e => e.CarId);
                    entity.HasIndex(e => e.UploadedAt);
                }
            );

            // Configure enum conversions to strings
            modelBuilder.Entity<CarItem>()
                .Property(e => e.Fuel)
                .HasConversion<string>();
            
            modelBuilder.Entity<CarItem>()
                .Property(e => e.Drive)
                .HasConversion<string>();
            
            modelBuilder.Entity<CarItem>()
                .Property(e => e.Transmission)
                .HasConversion<string>();
            
            modelBuilder.Entity<CarItem>()
                .Property(e => e.Body)
                .HasConversion<string>();
            
            modelBuilder.Entity<CarItem>()
                .Property(e => e.Condition)
                .HasConversion<string>();
        }
    }
}
