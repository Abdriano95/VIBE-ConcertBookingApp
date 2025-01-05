using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;
using DMA_AU24_LAB2_Group4.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DMA_AU24_LAB2_Group4.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // POST: api/Customer/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCustomerDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Control if email already exists
                if (await _unitOfWork.Customers.EmailExistsAsync(registerDto.Email))
                {
                    return BadRequest("Email is already in use.");
                }

                // Map DTO to Entity
                var customer = _mapper.Map<Customer>(registerDto);

                // Create customer
                await _unitOfWork.Customers.AddCustomerAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                // Return the created customer
                var customerDto = _mapper.Map<CustomerDto>(customer);
                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Could not register customer", Message = ex.Message });
            }
        }

        // POST: api/Customer/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Get customer by email
                var customer = await _unitOfWork.Customers.GetCustomerByEmailAsync(loginDto.Email);

                if (customer == null || customer.Password != loginDto.Password) // Enkel lösenordskontroll
                {
                    return Unauthorized("Invalid email or password.");
                }

                // Create and return customer DTO
                var customerDto = _mapper.Map<CustomerDto>(customer);
                return Ok(customerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Could not log in", Message = ex.Message });
            }
        }

        // GET: api/Customer/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetCustomerByIdAsync(id);
                if (customer == null)
                    return NotFound();

                var customerDto = _mapper.Map<CustomerDto>(customer);
                return Ok(customerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Could not retrieve customer", Message = ex.Message });
            }
        }

        // GET: api/Customer/{id}/bookings
        [HttpGet("{id}/bookings")]
        public async Task<IActionResult> GetCustomerBookings(int id)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetCustomerByIdAsync(id);
                if (customer == null)
                    return NotFound();
                var bookings = await _unitOfWork.Bookings.GetAllBookingsByCustomerIdAsync(id);
                var bookingDtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
                return Ok(bookingDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = "Could not retrieve customer bookings", Message = ex.Message });
            }
        }
    }
}
