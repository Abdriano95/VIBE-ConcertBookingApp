using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.Models
{
    public class Performance
    {
        public int Id { get; set; } = 0!;
        public DateTime PerformanceDateAndTime { get; set; }
        public string Venue { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public int ConcertId { get; set; } = 0!;

        //Navigation properties
        public Concert? Concert { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}
