using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMA_AU24_LAB2_Group4.Data.Entity
{
    public class Performance
    {
        [Key]
        public required int Id { get; set; }
        [Required]
        public required DateTime PerformanceDateAndTime { get; set; }
        [Required]
        public required string Venue { get; set; }
        [Required]
        public required string City { get; set; }
        [Required]
        public required string Country { get; set; }
        [ForeignKey(nameof(Concert))]
        public required int ConcertId { get; set; }

        //Navigation properties
        public Concert? Concert { get; set; }

        public ICollection<Booking>? Bookings { get; set; } = new List<Booking>();

    }
}
