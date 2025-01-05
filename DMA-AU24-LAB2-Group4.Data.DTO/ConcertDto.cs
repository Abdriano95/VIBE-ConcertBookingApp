using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.Data.DTO
{
    public class ConcertDto
    {
        public int ConcertId { get; set; } = 0!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        //public IEnumerable<PerformanceDto>? Performances { get; set; }

        // Initialize as an empty list by default to avoid null issues
        public IEnumerable<PerformanceDto> Performances { get; set; } = new List<PerformanceDto>();
    }
}
