using Microsoft.EntityFrameworkCore;

namespace ResteurantApi.Entities
{

    public class ResteurantDBContext : DbContext
    {
        //konfiugracja polaczenia z baza danych

        private string _connectionString = "Server=DESKTOP-T2TJMGJ;Database=ResteuranDb;Trusted_Connection=True;";

        public DbSet<Resteurant> Resteurants { get; set; }  
        public DbSet<Address> Address { get; set; }
        public DbSet<Dish> Dishes { get; set; }

        public DbSet<Role> Role { get; set; }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .Property(u => u.Name)
                .IsRequired();


            //konfiguracja encji
            modelBuilder.Entity<Resteurant>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(25);

            //ustawienie klucza obcego
           // modelBuilder.Entity<Address>()
             //  .HasOne(a => a.Resteurant)
               //.WithOne(b => b.Address)
               //.HasForeignKey<Resteurant>(b => b.AddressId);

            modelBuilder.Entity<Dish>()
                .Property(d => d.Name)
                .IsRequired();


            //zmiany wielkosci adresu
            modelBuilder.Entity<Address>()
                .Property(a => a.City)
                .IsRequired()
                .HasMaxLength(60);

            modelBuilder.Entity<Address>()
               .Property(a => a.Street)
               .IsRequired()
               .HasMaxLength(60);
        }

        //wywolujemy metode ktora łaczy nas z baza danych 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
