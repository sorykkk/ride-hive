using Microsoft.EntityFrameworkCore;
using Counter.Model;

namespace Counter.Data
{
    public class AppDbContext : DbContext //Prind DbContext C# vede baza de date
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Counter.Model.CounterClass> CounterClass { get; set; } //DbSet devine un tabel in baza de date
        public DbSet<Counter.Model.TestClass> TestClass { get; set; }

        // Aici configurăm modelul înainte de crearea tabelelor
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Spune EF Core că TestClass nu are PRIMARY KEY
            modelBuilder.Entity<Counter.Model.TestClass>().HasNoKey();

            // CounterClass nu trebuie modificată, EF Core știe că Id e PK ... 
        }
    }
}
