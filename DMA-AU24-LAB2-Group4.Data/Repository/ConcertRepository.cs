using DMA_AU24_LAB2_Group4.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.Data.Repository
{
    public class ConcertRepository : Repository<Concert>, IConcertRepository
    {
        public ApplicationDbContext DbContext => Context as ApplicationDbContext;

        public ConcertRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Concert>> GetAllConcertsAsync()
        {
            return await DbContext.Concerts
                .Include(c => c.Performances) // Include related performances
                .ToListAsync();
        }

        public async Task<Concert> GetConcertAsync(int id)
        {
            // Fetch the concert by its primary key and include performances
            return await DbContext.Concerts
                .Include(c => c.Performances) // Include performances to load related data
                .FirstOrDefaultAsync(c => c.Id == id); // Match the primary key
        }
    }
}
