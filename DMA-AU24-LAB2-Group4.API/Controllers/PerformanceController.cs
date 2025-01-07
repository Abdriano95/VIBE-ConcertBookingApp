using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;
using DMA_AU24_LAB2_Group4.Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace DMA_AU24_LAB2_Group4.API.Controllers
{
    public enum PerformanceErrorCode
    {
        PerformanceIDNotFound,
        InvalidPerformanceId
    }

    [ApiController]
    [Route("api/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PerformanceController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Performance
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var performances = await _unitOfWork.Performances.GetAllPerformancesAsync();
            return Ok(_mapper.Map<IEnumerable<PerformanceDto>>(performances));
        }

        // GET: api/Performance/concert/{id}
        [HttpGet("concert/{id}")]
        public async Task<IActionResult> GetByConcertId(int id)
        {
            var performances = await _unitOfWork.Performances.GetPerformancesByConcertIdAsync(id);

            if (!performances.Any())
            {
                return NotFound(PerformanceErrorCode.PerformanceIDNotFound.ToString());
            }

            return Ok(_mapper.Map<IEnumerable<PerformanceDto>>(performances));
        }

        // GET: api/Performance/available/{concertId}/{customerId}
        [HttpGet("available/{concertId}/{customerId}")]
        public async Task<IActionResult> GetAvailablePerformances(int concertId, int customerId)
        {
            var performances = await _unitOfWork.Performances.GetAvailablePerformancesForCustomerAsync(concertId, customerId);
            return Ok(_mapper.Map<IEnumerable<PerformanceDto>>(performances));
        }

        // GET: api/Performance/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerformanceById(int id)
        {
            var performance = await _unitOfWork.Performances.GetPerformanceByIdAsync(id);

            if (performance == null)
            {
                return NotFound(PerformanceErrorCode.PerformanceIDNotFound.ToString());
            }

            return Ok(_mapper.Map<PerformanceDto>(performance));
        }



    }
}
