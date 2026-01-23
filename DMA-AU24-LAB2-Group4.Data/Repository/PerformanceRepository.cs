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
        public ApplicationDbContext DbContext => (ApplicationDbContext)Context;

        public PerformanceRepository(ApplicationDbContext context)
            : base(context) { }



        //get all performances, include concerts and bookings
        public async Task<IEnumerable<Performance>> GetAllPerformancesAsync()
        {
            return await DbContext.Performances
                .Include(p => p.Concert)
                .Include(p => p.Bookings)
                .ToListAsync();
        }




        // Get all performances that are not booked by the customer
        public async Task<IEnumerable<Performance>> GetAvailablePerformancesForCustomerAsync(int concertId, int customerId)
        {
            return await DbContext.Performances
                            .Where(p => p.ConcertId == concertId &&
                                        (p.Bookings == null || !p.Bookings.Any(b => b.CustomerId == customerId)))
                            .Include(p => p.Concert)
                            .Include(p => p.Bookings) 
                            .ToListAsync();
        }


        // get all peroformances by concert id
        public async Task<IEnumerable<Performance>> GetPerformancesByConcertIdAsync(int concertId)
        {
            return await DbContext.Performances
                .Where(p => p.ConcertId == concertId)
                .Include(p => p.Concert)
                .Include(p => p.Bookings)
                .ToListAsync();
        }

        // get speicific performance by id
        public async Task<Performance?> GetPerformanceByIdAsync(int id)
        {
            return await DbContext.Performances
                .Include(p => p.Concert)
                .Include(p => p.Bookings)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
