using DMA_AU24_LAB2_Group4.Data;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;
using DMA_AU24_LAB2_Group4.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DMA_AU24_LAB2_Group4.Test
{
    public class BookingTests
    {
        public class BookingContext : DbContext
        { 
            public BookingContext(DbContextOptions<BookingContext> options) : base(options)
            {
            }

            public DbSet<BookingCreateDto> Bookings { get; set; }
        }

        [Fact]
        public async Task AddBooking_ShouldAddBookingToDatabase()
        {
            // Arrange - Use unique database name to avoid conflicts between test runs
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                // Setup required related entities
                var customer = new Customer
                {
                    Id = 1,
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@example.com",
                    Password = "hashedpassword"
                };

                var concert = new Concert
                {
                    Id = 1,
                    Title = "Test Concert",
                    Description = "Test Description"
                };

                var performance = new Performance
                {
                    Id = 1,
                    ConcertId = 1,
                    Venue = "Test Venue",
                    City = "Test City",
                    Country = "Test Country",
                    PerformanceDateAndTime = DateTime.Now.AddDays(30),
                    Concert = concert
                };

                context.Customers.Add(customer);
                context.Concerts.Add(concert);
                context.Performances.Add(performance);
                await context.SaveChangesAsync();

                var unitOfWork = new UnitOfWork(context);
                var booking = new Booking
                {
                    Id = 1,
                    CustomerId = 1,
                    PerformanceId = 1
                };

                // Act
                context.Bookings.Add(booking);
                var saveResult = await unitOfWork.SaveChangesAsync();

                // Assert
                Assert.Equal(1, saveResult); // Ensure one record was saved

                var savedBookings = await unitOfWork.Bookings.GetAllBookingDetailsByIdAsync(booking.Id);
                Assert.NotNull(savedBookings);
                Assert.NotEmpty(savedBookings);
                
                var savedBooking = savedBookings.First();
                Assert.Equal(booking.CustomerId, savedBooking.CustomerId);
                Assert.Equal(booking.PerformanceId, savedBooking.PerformanceId);
                Assert.NotNull(savedBooking.Customer);
                Assert.NotNull(savedBooking.Performance);
                Assert.Equal("Test", savedBooking.Customer.FirstName);
                Assert.Equal("Test Venue", savedBooking.Performance.Venue);
            }
        }
    }
}
