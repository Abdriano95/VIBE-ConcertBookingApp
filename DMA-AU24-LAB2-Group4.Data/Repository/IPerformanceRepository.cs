using DMA_AU24_LAB2_Group4.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.Data.Repository
{
    public interface IPerformanceRepository : IRepository<Performance>
    {
        Task<IEnumerable<Performance>> GetAllPerformancesAsync();

        Task<IEnumerable<Performance>> GetAvailablePerformancesForCustomerAsync(int concertId, int customerId);

        Task<IEnumerable<Performance>> GetPerformancesByConcertIdAsync(int concertId);

        Task<Performance?> GetPerformanceByIdAsync(int id);
    }
}
