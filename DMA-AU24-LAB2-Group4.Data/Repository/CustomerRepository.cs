using DMA_AU24_LAB2_Group4.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.Data.Repository
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public ApplicationDbContext DbContext => Context as ApplicationDbContext;

        public CustomerRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await GetAllAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await DbContext.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await AddAsync(customer);
        }

        public void UpdateCustomer(Customer customer)
        {
            DbContext.Customers.Update(customer);
        }

        public void DeleteCustomer(Customer customer)
        {
            Delete(customer);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await DbContext.Customers.AnyAsync(c => c.Email == email);
        }
    }
}
