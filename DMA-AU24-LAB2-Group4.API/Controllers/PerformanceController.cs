using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;
using DMA_AU24_LAB2_Group4.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DMA_AU24_LAB2_Group4.API.Controllers
{
    /// <summary>
    /// Manages performance operations including retrieving performance schedules and availability.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PerformanceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the PerformanceController.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The AutoMapper instance for DTO mapping.</param>
        public PerformanceController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all performances across all concerts.
        /// </summary>
        /// <returns>A list of all performances in the system.</returns>
        /// <response code="200">Returns the list of all performances.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PerformanceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            var performances = await _unitOfWork.Performances.GetAllPerformancesAsync();
            return Ok(_mapper.Map<IEnumerable<PerformanceDto>>(performances));
        }

        /// <summary>
        /// Retrieves all performances for a specific concert.
        /// </summary>
        /// <param name="id">The unique identifier of the concert.</param>
        /// <returns>A list of performances for the specified concert.</returns>
        /// <response code="200">Returns the list of performances for the concert.</response>
        /// <response code="404">If no performances are found for the specified concert.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("concert/{id}")]
        [ProducesResponseType(typeof(IEnumerable<PerformanceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByConcertId(int id)
        {
            var performances = await _unitOfWork.Performances.GetPerformancesByConcertIdAsync(id);

            if (!performances.Any())
            {
                return NotFound(new { Error = "NoPerformancesFound", Message = $"No performances found for concert ID {id}." });
            }

            return Ok(_mapper.Map<IEnumerable<PerformanceDto>>(performances));
        }

        /// <summary>
        /// Retrieves performances available for booking by a specific customer for a specific concert.
        /// </summary>
        /// <param name="concertId">The unique identifier of the concert.</param>
        /// <param name="customerId">The unique identifier of the customer.</param>
        /// <returns>A list of performances the customer hasn't booked yet.</returns>
        /// <remarks>
        /// This endpoint filters out performances that the customer has already booked,
        /// showing only available performances they can still book.
        /// </remarks>
        /// <response code="200">Returns the list of available performances (may be empty if all are booked).</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("available/{concertId}/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<PerformanceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailablePerformances(int concertId, int customerId)
        {
            var performances = await _unitOfWork.Performances.GetAvailablePerformancesForCustomerAsync(concertId, customerId);
            return Ok(_mapper.Map<IEnumerable<PerformanceDto>>(performances));
        }

        /// <summary>
        /// Retrieves a specific performance by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the performance.</param>
        /// <returns>The performance details if found.</returns>
        /// <response code="200">Returns the performance with the specified ID.</response>
        /// <response code="404">If no performance is found with the specified ID.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PerformanceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPerformanceById(int id)
        {
            var performance = await _unitOfWork.Performances.GetPerformanceByIdAsync(id);

            if (performance == null)
            {
                return NotFound(new { Error = "PerformanceNotFound", Message = $"No performance found with ID {id}." });
            }

            return Ok(_mapper.Map<PerformanceDto>(performance));
        }
    }
}
