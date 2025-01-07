using System.ComponentModel.DataAnnotations;

namespace DMA_AU24_LAB2_Group4.Data.Entity
{
    public class Customer
    {
        [Key]
        public required int Id { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(20)]
        public required string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(20)]
        public required string LastName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(50)]
        public required string Email { get; set; }
        [Required]
        [StringLength(60, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 60 characters.")]
        public required string Password { get; set; }

        //Navigation properties
        public ICollection<Booking> Bookings = new List<Booking>();
    }
}
