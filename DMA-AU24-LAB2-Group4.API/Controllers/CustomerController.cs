using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;
using DMA_AU24_LAB2_Group4.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DMA_AU24_LAB2_Group4.API.Controllers
{
    /// <summary>
    /// Manages customer operations including registration, authentication, profile management, and booking retrieval.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CustomerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the CustomerController.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The AutoMapper instance for DTO mapping.</param>
        public CustomerController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Registers a new customer account.
        /// </summary>
        /// <param name="registerDto">The registration data containing customer details and password.</param>
        /// <returns>The newly created customer profile.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Customer/register
        ///     {
        ///         "firstName": "John",
        ///         "lastName": "Doe",
        ///         "email": "john.doe@example.com",
        ///         "password": "SecurePassword123!"
        ///     }
        /// 
        /// Notes:
        /// - Email addresses must be unique across all customers.
        /// - Passwords are securely hashed using BCrypt before storage.
        /// </remarks>
        /// <response code="201">Returns the newly created customer.</response>
        /// <response code="400">If the request is invalid or email is already in use.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterCustomerDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Check if email already exists
                if (await _unitOfWork.Customers.EmailExistsAsync(registerDto.Email))
                {
                    return BadRequest(new { Error = "EmailInUse", Message = "This email address is already registered." });
                }

                // Map DTO to Entity
                var customer = _mapper.Map<Customer>(registerDto);

                // Hash the password before storing
                customer.Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // Create customer
                await _unitOfWork.Customers.AddCustomerAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                // Return the created customer
                var customerDto = _mapper.Map<CustomerDto>(customer);
                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customerDto);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "RegistrationFailed", Message = "Could not register customer. Please try again." });
            }
        }

        /// <summary>
        /// Authenticates a customer and returns their profile information.
        /// </summary>
        /// <param name="loginDto">The login credentials containing email and password.</param>
        /// <returns>The customer profile if authentication is successful.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Customer/login
        ///     {
        ///         "email": "john.doe@example.com",
        ///         "password": "SecurePassword123!"
        ///     }
        /// 
        /// Notes:
        /// - Password verification uses BCrypt secure comparison.
        /// - Invalid credentials return a generic error message to prevent user enumeration.
        /// </remarks>
        /// <response code="200">Returns the customer profile on successful authentication.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="401">If authentication fails (invalid email or password).</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Get customer by email
                var customer = await _unitOfWork.Customers.GetCustomerByEmailAsync(loginDto.Email);

                if (customer == null)
                {
                    return Unauthorized(new { Error = "InvalidCredentials", Message = "Invalid email or password." });
                }

                // Verify password using BCrypt
                bool isPasswordValid;
                try
                {
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, customer.Password);
                }
                catch (BCrypt.Net.SaltParseException)
                {
                    // Invalid hash format - treat as authentication failure
                    return Unauthorized(new { Error = "InvalidCredentials", Message = "Invalid email or password." });
                }

                if (!isPasswordValid)
                {
                    return Unauthorized(new { Error = "InvalidCredentials", Message = "Invalid email or password." });
                }

                // Create and return customer DTO
                var customerDto = _mapper.Map<CustomerDto>(customer);
                return Ok(customerDto);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "LoginFailed", Message = "Could not complete login. Please try again." });
            }
        }

        /// <summary>
        /// Retrieves a customer profile by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the customer.</param>
        /// <returns>The customer profile if found.</returns>
        /// <response code="200">Returns the customer with the specified ID.</response>
        /// <response code="404">If no customer is found with the specified ID.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return NotFound(new { Error = "CustomerNotFound", Message = $"No customer found with ID {id}." });
                }

                var customerDtoResult = _mapper.Map<CustomerDto>(customer);
                return Ok(customerDtoResult);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "RetrievalFailed", Message = "Could not retrieve customer. Please try again." });
            }
        }

        /// <summary>
        /// Retrieves all bookings for a specific customer.
        /// </summary>
        /// <param name="customerDto">The customer data containing the customer ID.</param>
        /// <returns>A list of bookings belonging to the specified customer.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/Customer/getBookings
        ///     {
        ///         "customerID": 1
        ///     }
        /// 
        /// </remarks>
        /// <response code="200">Returns the list of bookings for the customer.</response>
        /// <response code="400">If the request data is invalid.</response>
        /// <response code="404">If the customer doesn't exist.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("getBookings")]
        [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerBookings([FromBody] CustomerDto customerDto)
        {
            if (customerDto == null || customerDto.CustomerID <= 0)
            {
                return BadRequest(new { Error = "InvalidRequest", Message = "Valid customer ID is required." });
            }

            try
            {
                var customer = await _unitOfWork.Customers.GetCustomerByIdAsync(customerDto.CustomerID);
                if (customer == null)
                    return NotFound(new { Error = "CustomerNotFound", Message = $"No customer found with ID {customerDto.CustomerID}." });

                var bookings = await _unitOfWork.Bookings.GetAllBookingsByCustomerIdAsync(customerDto.CustomerID);
                var bookingDtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
                return Ok(bookingDtos);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "RetrievalFailed", Message = "Could not retrieve customer bookings. Please try again." });
            }
        }

        /// <summary>
        /// Updates an existing customer's profile information.
        /// </summary>
        /// <param name="updateDto">The updated customer data.</param>
        /// <returns>A success message if the update is successful.</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/Customer/update
        ///     {
        ///         "id": 1,
        ///         "firstName": "John",
        ///         "lastName": "Smith",
        ///         "email": "john.smith@example.com",
        ///         "password": "NewPassword123!"
        ///     }
        /// 
        /// Notes:
        /// - If password is empty or null, the existing password is preserved.
        /// - New passwords are securely hashed using BCrypt before storage.
        /// </remarks>
        /// <response code="200">The profile was successfully updated.</response>
        /// <response code="404">If no customer is found with the specified ID.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPut("update")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomer([FromBody] UpdateCustomerDto updateDto)
        {
            try
            {
                var customer = await _unitOfWork.Customers.GetCustomerByIdAsync(updateDto.Id);
                if (customer == null)
                    return NotFound(new { Error = "CustomerNotFound", Message = $"No customer found with ID {updateDto.Id}." });

                // Store the current hashed password
                var currentHashedPassword = customer.Password;

                // Update customer fields from DTO
                _mapper.Map(updateDto, customer);

                // If a new password is provided, hash it; otherwise, keep the existing hashed password
                if (!string.IsNullOrWhiteSpace(updateDto.Password))
                {
                    customer.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
                }
                else
                {
                    customer.Password = currentHashedPassword;
                }

                _unitOfWork.Customers.UpdateCustomer(customer);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new { Message = "Profile updated successfully." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "UpdateFailed", Message = "Could not update profile. Please try again." });
            }
        }
    }
}
