using DMA_AU24_LAB2_Group4.MAUI.Models;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    public interface ICustomerService
    {
        Task<bool> RegisterCustomerAsync(Customer customer);
        Task<Customer?> LoginAsync(string email, string password);
        Task<Customer?> GetProfileAsync();
        Task<bool> UpdateProfileAsync(Customer customer);
    }
}