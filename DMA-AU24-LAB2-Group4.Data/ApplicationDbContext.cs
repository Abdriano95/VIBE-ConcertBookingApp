using DMA_AU24_LAB2_Group4.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace DMA_AU24_LAB2_Group4.Data
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<Concert> Concerts { get; set; } = null!;
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Performance> Performances { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Concert Entity
            modelBuilder.Entity<Concert>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd(); // Auto-generate IDs
                entity.Property(c => c.Title)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(c => c.Description)
                    .IsRequired()
                    .HasMaxLength(500);
                entity.Property(c => c.ImageUrl)
                    .HasMaxLength(500);

                entity.HasMany(c => c.Performances)
                    .WithOne(p => p.Concert)
                    .HasForeignKey(p => p.ConcertId)
                    .OnDelete(DeleteBehavior.Cascade); // Cascade delete performances when a concert is deleted
            });

            // Configure Performance Entity
            modelBuilder.Entity<Performance>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).ValueGeneratedOnAdd();
                entity.Property(p => p.PerformanceDateAndTime)
                    .IsRequired();
                entity.Property(p => p.Venue)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(p => p.City)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(p => p.Country)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasMany(p => p.Bookings)
                    .WithOne(b => b.Performance)
                    .HasForeignKey(b => b.PerformanceId)
                    .OnDelete(DeleteBehavior.Cascade); // Cascade delete bookings when a performance is deleted
            });

            // Configure Customer Entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();
                entity.Property(c => c.FirstName)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(c => c.LastName)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(c => c.Password)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.HasMany(c => c.Bookings)
                    .WithOne(b => b.Customer)
                    .HasForeignKey(b => b.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict); // Restrict delete if customer has bookings
            });

            // Configure Booking Entity
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);
                entity.Property(b => b.Id).ValueGeneratedOnAdd();

                entity.HasOne(b => b.Performance)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(b => b.PerformanceId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.Customer)
                    .WithMany(c => c.Bookings)
                    .HasForeignKey(b => b.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict); // Restrict delete if bookings exist
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder builder)
        {
            // Seed Concerts with artist images from Unsplash
            Concert concert1 = new()
            {
                Id = 1,
                Title = "Ed Sheeran World Tour",
                Description = "Experience the best of Ed Sheeran live!",
                ImageUrl = "https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=800",
                Performances = new List<Performance>()
            };

            Concert concert2 = new()
            {
                Id = 2,
                Title = "Taylor Swift Eras Tour",
                Description = "A musical journey through Taylor Swift's iconic albums.",
                ImageUrl = "https://images.unsplash.com/photo-1501386761578-eac5c94b800a?w=800",
                Performances = new List<Performance>()
            };

            Concert concert3 = new()
            {
                Id = 3,
                Title = "Kendrick Lamar GNX Summer World Tour",
                Description = "Kendrick in his absolute prime, destroying his enemies.",
                ImageUrl = "https://images.unsplash.com/photo-1598387993441-a364f854c3e1?w=800",
                Performances = new List<Performance>()
            };

            Concert concert4 = new()
            {
                Id = 4,
                Title = "Bruno Mars Die With A Smile World Tour",
                Description = "Bruno Mars isn't just a performer; he's a showman. " +
                "His world tours are legendary, delivering unforgettable experiences that blend incredible musicianship, dazzling choreography, and a pure party atmosphere.",
                ImageUrl = "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3?w=800",
                Performances = new List<Performance>()
            };

            Concert concert5 = new()
            {
                Id = 5,
                Title = "Beyoncé Renaissance World Tour",
                Description = "A celebration of Beyoncé's iconic Renaissance album.",
                ImageUrl = "https://images.unsplash.com/photo-1459749411175-04bf5292ceea?w=800",
                Performances = new List<Performance>()
            };

            // Seed Performances
            Performance performance1 = new()
            {
                Id = 1,
                PerformanceDateAndTime = new DateTime(2025, 6, 1, 19, 0, 0),
                Venue = "Madison Square Garden",
                City = "New York",
                Country = "USA",
                ConcertId = 1
            };

            Performance performance2 = new()
            {
                Id = 2,
                PerformanceDateAndTime = new DateTime(2025, 6, 3, 20, 0, 0),
                Venue = "Staples Center",
                City = "Los Angeles",
                Country = "USA",
                ConcertId = 1
            };

            Performance performance3 = new()
            {
                Id = 3,
                PerformanceDateAndTime = new DateTime(2025, 6, 5, 18, 0, 0),
                Venue = "Globen",
                City = "Stockholm",
                Country = "Sweden",
                ConcertId = 1
            };

            Performance performance4 = new()
            {
                Id = 4,
                PerformanceDateAndTime = new DateTime(2025, 7, 10, 18, 0, 0),
                Venue = "Wembley Stadium",
                City = "London",
                Country = "UK",
                ConcertId = 2
            };

            Performance performance5 = new()
            {
                Id = 5,
                PerformanceDateAndTime = new DateTime(2025, 7, 12, 19, 0, 0),
                Venue = "O2 Arena",
                City = "London",
                Country = "UK",
                ConcertId = 2
            };

            Performance performance6 = new()
            {
                Id = 6,
                PerformanceDateAndTime = new DateTime(2025, 8, 15, 20, 0, 0),
                Venue = "Olympiastadion",
                City = "Berlin",
                Country = "Germany",
                ConcertId = 2
            };

            Performance performance7 = new()
            {
                Id = 7,
                PerformanceDateAndTime = new DateTime(2025, 8, 17, 20, 0, 0),
                Venue = "Stade de France",
                City = "Paris",
                Country = "France",
                ConcertId = 3
            };

            Performance performance8 = new()
            {
                Id = 8,
                PerformanceDateAndTime = new DateTime(2025, 9, 20, 19, 0, 0),
                Venue = "Estadio Azteca",
                City = "Mexico City",
                Country = "Mexico",
                ConcertId = 3
            };

            Performance performance9 = new()
            {
                Id = 9,
                PerformanceDateAndTime = new DateTime(2025, 9, 22, 20, 0, 0),
                Venue = "Estadio Monumental",
                City = "Santiago",
                Country = "Chile",
                ConcertId = 3
            };

            Performance performance10 = new()
            {
                Id = 10,
                PerformanceDateAndTime = new DateTime(2025, 10, 22, 20, 0, 0),
                Venue = "San Siro",
                City = "Milano",
                Country = "Italy",
                ConcertId = 4
            };

            Performance performance11 = new()
            {
                Id = 11,
                PerformanceDateAndTime = new DateTime(2025, 10, 24, 20, 0, 0),
                Venue = "Stadio Olimpico",
                City = "Rome",
                Country = "Italy",
                ConcertId = 4
            };

            Performance performance12 = new()
            {
                Id = 12,
                PerformanceDateAndTime = new DateTime(2025, 11, 22, 20, 0, 0),
                Venue = "Estádio da Luz",
                City = "Lisbon",
                Country = "Portugal",
                ConcertId = 5
            };

            Performance performance13 = new()
            {
                Id = 13,
                PerformanceDateAndTime = new DateTime(2025, 11, 24, 20, 0, 0),
                Venue = "Estadi Olímpic Lluís Companys",
                City = "Barcelona",
                Country = "Spain",
                ConcertId = 5
            };

            Performance performance14 = new()
            {
                Id = 14,
                PerformanceDateAndTime = new DateTime(2025, 12, 22, 20, 0, 0),
                Venue = "Estádio do Dragão",
                City = "Porto",
                Country = "Portugal",
                ConcertId = 5
            };

            // Seed Customers
            // Note: Passwords are BCrypt hashed. Plain-text for testing: customer1 = "Password123!", customer2 = "Password456!"
            // Pre-computed hashes ensure consistent seed data across migrations
            Customer customer1 = new()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "$2a$11$14iUvy7JDmzV14JpiRZmA.czCYnNgnrGnEG9XNuKzxyIaV/jWRTDi" // Password123!
            };
            Customer customer2 = new()
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Password = "$2a$11$uxpheqSZiEIy10yyL7a8GOhXV4r7XXPyHik89046f3h3OI.Ldwspe" // Password456!
            };

            // Seed Bookings
            Booking booking1 = new()
            {
                Id = 1,
                PerformanceId = 1,
                CustomerId = 1
            };
            Booking booking2 = new()
            {
                Id = 2,
                PerformanceId = 3,
                CustomerId = 2
            };

            Booking booking3 = new()
            {
                Id = 3,
                PerformanceId = 4,
                CustomerId = 1
            };

            Booking booking4 = new()
            {
                Id = 4,
                PerformanceId = 6,
                CustomerId = 2
            };

            // Add the data to the ModelBuilder
            builder.Entity<Concert>().HasData(concert1, concert2, concert3, concert4, concert5);
            builder.Entity<Performance>().HasData(performance1, performance2, performance3, performance4, performance5, performance6, performance7, performance8, performance9, performance10, performance11, performance12, performance13, performance14);
            builder.Entity<Customer>().HasData(customer1, customer2);
            builder.Entity<Booking>().HasData(booking1, booking2, booking3, booking4);
        }
    }
}
