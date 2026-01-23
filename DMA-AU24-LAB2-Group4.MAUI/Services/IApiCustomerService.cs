using DMA_AU24_LAB2_Group4.MAUI.Models;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    /// <summary>
    /// Service interface for customer-related API operations.
    /// </summary>
    public interface IApiCustomerService
    {
        /// <summary>
        /// Registers a new customer.
        /// </summary>
        Task<bool> RegisterCustomerAsync(Customer customer);

        /// <summary>
        /// Authenticates a customer with email and password.
        /// </summary>
        Task<Customer?> LoginAsync(string email, string password);

        /// <summary>
        /// Gets a customer's profile by ID.
        /// </summary>
        Task<Customer?> GetProfileAsync(int customerId);

        /// <summary>
        /// Updates a customer's profile.
        /// </summary>
        Task<bool> UpdateProfileAsync(Customer customer);
    }
}
