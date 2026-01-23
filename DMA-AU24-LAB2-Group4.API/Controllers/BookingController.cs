using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;
using DMA_AU24_LAB2_Group4.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DMA_AU24_LAB2_Group4.API.Controllers
{
    /// <summary>
    /// Manages concert booking operations including creating, retrieving, and deleting bookings.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BookingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the BookingController.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The AutoMapper instance for DTO mapping.</param>
        public BookingController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all bookings with full details including customer and performance information.
        /// </summary>
        /// <returns>A list of all bookings in the system.</returns>
        /// <response code="200">Returns the list of all bookings.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            return Ok(_mapper.Map<IEnumerable<BookingDto>>(await _unitOfWork.Bookings.GetAllBookingDetailsAsync()));
        }

        /// <summary>
        /// Retrieves a specific booking by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the booking.</param>
        /// <returns>The booking details if found.</returns>
        /// <response code="200">Returns the booking with the specified ID.</response>
        /// <response code="404">If no booking is found with the specified ID.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListById(int id)
        {
            var booking = await _unitOfWork.Bookings.GetAllBookingDetailsByIdAsync(id);

            if (booking == null || !booking.Any())
            {
                return NotFound(new { Error = "BookingNotFound", Message = $"No booking found with ID {id}." });
            }

            var bookingDto = _mapper.Map<BookingDto>(booking.First());
            return Ok(bookingDto);
        }

        /// <summary>
        /// Retrieves all bookings for a specific customer.
        /// </summary>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <returns>A list of bookings belonging to the specified customer.</returns>
        /// <response code="200">Returns the list of bookings for the customer.</response>
        /// <response code="404">If the customer has no bookings or doesn't exist.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListByCustomerId(int customerId)
        {
            var bookings = await _unitOfWork.Bookings.GetAllBookingsByCustomerIdAsync(customerId);

            if (bookings == null || !bookings.Any())
            {
                return NotFound(new { Error = "NoBookingsFound", Message = $"No bookings found for customer ID {customerId}." });
            }
            return Ok(_mapper.Map<IEnumerable<BookingDto>>(bookings));
        }

        /// <summary>
        /// Creates a new booking for a customer and performance.
        /// </summary>
        /// <param name="bookingDto">The booking creation data containing customer and performance IDs.</param>
        /// <returns>The created booking details.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Booking
        ///     {
        ///         "customerId": 1,
        ///         "performanceId": 1
        ///     }
        /// 
        /// Notes:
        /// - A customer can only have one booking per performance.
        /// - Both the customer and performance must exist in the system.
        /// </remarks>
        /// <response code="200">Returns the newly created booking.</response>
        /// <response code="400">If the request is invalid, customer/performance doesn't exist, or booking already exists.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost]
        [ProducesResponseType(typeof(BookingCreateDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
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
                    ModelState.AddModelError("Booking", "Booking already exists for this customer and performance");
                    return BadRequest(ModelState);
                }

                // Create a new Booking Entity
                booking = new Booking
                {
                    Id = booking.Id,
                    CustomerId = booking.CustomerId,
                    PerformanceId = booking.PerformanceId
                };

                // Add and save the Booking
                await _unitOfWork.Bookings.AddAsync(booking);
                await _unitOfWork.SaveChangesAsync();

            }
            catch (Exception)
            {
                return BadRequest(new { Error = "CouldNotCreateBooking", Message = "Failed to create the booking. Please try again." });
            }
            return Ok(_mapper.Map<BookingCreateDto>(booking));
        }

        /// <summary>
        /// Deletes a booking by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the booking to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">The booking was successfully deleted.</response>
        /// <response code="404">If no booking is found with the specified ID.</response>
        /// <response code="400">If the deletion fails due to a constraint or other error.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Ensure the Booking exists
                Booking? booking = await _unitOfWork.Bookings.GetByIdAsync(id);
                if (booking is null)
                    return NotFound(new { Error = "BookingNotFound", Message = $"No booking found with ID {id}." });

                _unitOfWork.Bookings.Delete(id);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest(new { Error = "CouldNotDeleteBooking", Message = "Failed to delete the booking. Please try again." });
            }
            return NoContent();
        }
    }
}
