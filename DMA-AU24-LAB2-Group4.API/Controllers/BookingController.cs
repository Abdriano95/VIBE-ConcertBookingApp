using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;
using DMA_AU24_LAB2_Group4.Data.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DMA_AU24_LAB2_Group4.API.Controllers
{
    public enum ErrorCode
    {
        BookingIDInUse,
        BookingIDNotFound,
        RecordNotFound,
        CouldNotCreateBooking,
        CouldNotDeleteBooking,
        CouldNotUpdateBooking,
    }
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            return Ok(_mapper.Map<IEnumerable<BookingDto>>(await _unitOfWork.Bookings.GetAllBookingDetailsAsync()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ListById(int id)
        {
            var booking = await _unitOfWork.Bookings.GetAllBookingDetailsByIdAsync(id);

            // Control if the booking is null or empty
            if (booking == null || !booking.Any())
            {
                return NotFound(ErrorCode.BookingIDNotFound.ToString());
            }

            // Gets the first booking from the list and maps it to a BookingDto object
            var bookingDto = _mapper.Map<BookingDto>(booking.First());
            return Ok(bookingDto);
        }


        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> ListByCustomerId(int customerId)
        {
            var bookings = await _unitOfWork.Bookings.GetAllBookingsByCustomerIdAsync(customerId);

            // Check if no bookings were found for the provided Customer ID
            if (bookings == null || !bookings.Any())
            {
                return NotFound(ErrorCode.BookingIDNotFound.ToString());
            }
            return Ok(_mapper.Map<IEnumerable<BookingDto>>(bookings));
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookingCreateDto bookingDto)
        {
            Booking booking;
            try
            {
                booking = _mapper.Map<Booking>(bookingDto);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Ensure the Customer exists
                Customer? customer = await _unitOfWork.Customers.GetByIdAsync(booking.CustomerId);
                if (customer is null)
                {
                    ModelState.AddModelError("CustomerId", "Invalid Customer ID");
                    return BadRequest(ModelState);
                }

                // Ensure the Performance exists
                Performance? performance = await _unitOfWork.Performances.GetByIdAsync(booking.PerformanceId);
                if (performance is null)
                {
                    ModelState.AddModelError("PerformanceId", "Invalid Performance ID");
                    return BadRequest(ModelState);
                }

                // Ensure the Booking does not already exist
                Booking? existingBooking = await _unitOfWork.Bookings.FindByCustomerAndPerformanceAsync(booking.CustomerId, booking.PerformanceId);
                if (existingBooking is not null)
                {
                    ModelState.AddModelError("Booking", "Booking already exists");
                    return BadRequest(ModelState);
                }

                // Create a new Booking Entity
                booking = new Booking
                {
                    Id = booking.Id, // Set the required Id property
                    CustomerId = booking.CustomerId,
                    PerformanceId = booking.PerformanceId
                };

                // Add and save the Booking
                await _unitOfWork.Bookings.AddAsync(booking);
                await _unitOfWork.SaveChangesAsync();

            }
            catch (Exception)
            {
                return BadRequest(ErrorCode.CouldNotCreateBooking.ToString());
            }
            return Ok(_mapper.Map<BookingCreateDto>(booking));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Ensure the Booking exists
                Booking? booking = await _unitOfWork.Bookings.GetByIdAsync(id);
                if (booking is null)
                    return NotFound(ErrorCode.RecordNotFound.ToString());

                _unitOfWork.Bookings.Delete(id);
                await _unitOfWork.SaveChangesAsync();                
            }
            catch (Exception)
            {
                return BadRequest(ErrorCode.CouldNotDeleteBooking.ToString());
            }
            return NoContent();
        }
    }
}
