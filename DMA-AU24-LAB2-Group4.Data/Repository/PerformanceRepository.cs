using DMA_AU24_LAB2_Group4.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.Data.Repository
{
    public class PerformanceRepository : Repository<Performance>, IPerformanceRepository
    {
        public ApplicationDbContext DbContext => Context as ApplicationDbContext;

        public PerformanceRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Performance>> GetPerformancesByConcertIdAsync(int concertId)
        {
            return await DbContext.Set<Performance>()
                .Where(p => p.ConcertId == concertId)
                .ToListAsync();
        }


    }
}
