using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DMA_AU24_LAB2_Group4.API.Controllers
{
    /// <summary>
    /// Manages concert operations including retrieving concert listings and details.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ConcertController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the ConcertController.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for database operations.</param>
        /// <param name="mapper">The AutoMapper instance for DTO mapping.</param>
        public ConcertController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all available concerts.
        /// </summary>
        /// <returns>A list of all concerts in the system.</returns>
        /// <response code="200">Returns the list of all concerts.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ConcertDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            var concerts = await _unitOfWork.Concerts.GetAllConcertsAsync();
            return Ok(_mapper.Map<IEnumerable<ConcertDto>>(concerts));
        }

        /// <summary>
        /// Retrieves a specific concert by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the concert.</param>
        /// <returns>The concert details if found.</returns>
        /// <response code="200">Returns the concert with the specified ID.</response>
        /// <response code="404">If no concert is found with the specified ID.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ConcertDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            var concert = await _unitOfWork.Concerts.GetConcertAsync(id);

            if (concert == null)
            {
                return NotFound(new { Error = "ConcertNotFound", Message = $"No concert found with ID {id}." });
            }

            return Ok(_mapper.Map<ConcertDto>(concert));
        }
    }
}
