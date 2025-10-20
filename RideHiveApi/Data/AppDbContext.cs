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
        public DbSet<PostItem> PostItems { get; set; }
        public DbSet<Owner> Owners { get; set; }

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

            // Configure PostItem entity
            modelBuilder.Entity<PostItem>(entity =>
                {
                    entity.HasKey(e => e.PostId);
                    entity.Property(e => e.PostId).ValueGeneratedOnAdd();

                    // Configure relationship with Owner
                    entity.HasOne(p => p.Owner)
                        .WithMany(o => o.Posts)
                        .HasForeignKey(p => p.OwnerId)
                        .OnDelete(DeleteBehavior.Cascade);

                    // Configure relationship with Car (cascade delete when car is deleted)
                    entity.HasOne(p => p.Car)
                        .WithMany()
                        .HasForeignKey(p => p.CarId)
                        .OnDelete(DeleteBehavior.Cascade);

                    // Configure indexes
                    entity.HasIndex(e => e.OwnerId);
                    entity.HasIndex(e => e.CarId);
                    entity.HasIndex(e => e.PostedAt);
                }
            );

            // Configure Owner entity
            modelBuilder.Entity<Owner>(entity =>
                {
                    entity.HasKey(e => e.OwnerId);
                    
                    // Configure indexes
                    entity.HasIndex(e => e.Name);
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

            // Configure AvailableTimeSlots to be stored as JSON
            modelBuilder.Entity<PostItem>()
                .Property(e => e.AvailableTimeSlots)
                .HasConversion(
                    timeSlots => System.Text.Json.JsonSerializer.Serialize(timeSlots, new System.Text.Json.JsonSerializerOptions()),
                    json => System.Text.Json.JsonSerializer.Deserialize<List<DateTime>>(json, new System.Text.Json.JsonSerializerOptions()) ?? new List<DateTime>()
                );
        }
    }
}
