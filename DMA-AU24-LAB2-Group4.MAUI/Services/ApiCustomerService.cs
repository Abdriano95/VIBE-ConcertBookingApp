using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    /// <summary>
    /// Service for customer-related API operations.
    /// </summary>
    public class ApiCustomerService : HttpClientService, IApiCustomerService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ApiCustomerService> _logger;

        public ApiCustomerService(
            IHttpsClientHandlerService httpsClientHandlerService,
            IMapper mapper,
            ILogger<ApiCustomerService> logger) : base(httpsClientHandlerService)
        {
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<bool> RegisterCustomerAsync(Customer customer)
        {
            try
            {
                var dto = _mapper.Map<RegisterCustomerDto>(customer);
                Uri uri = new Uri(Constants.CustomerRegisterUrl);
                var response = await _httpClient.PostAsJsonAsync(uri, dto);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Registration failed with status {StatusCode}: {ErrorContent}",
                        response.StatusCode, errorContent);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in RegisterCustomerAsync");
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<Customer?> LoginAsync(string email, string password)
        {
            try
            {
                var loginDto = new LoginDto { Email = email, Password = password };
                Uri uri = new Uri(Constants.CustomerLoginUrl);
                var response = await _httpClient.PostAsJsonAsync(uri, loginDto);

                if (!response.IsSuccessStatusCode) return null;

                var customerDto = await response.Content.ReadFromJsonAsync<CustomerDto>(_serializerOptions);

                if (customerDto == null)
                {
                    _logger.LogWarning("Failed to deserialize CustomerDto during login");
                    return null;
                }

                return _mapper.Map<Customer>(customerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in LoginAsync");
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<Customer?> GetProfileAsync(int customerId)
        {
            Uri uri = new Uri($"{Constants.BaseUrl}/customer/{customerId}");
            _logger.LogDebug("Fetching profile for CustomerId {CustomerId}", customerId);

            try
            {
                var response = await _httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var customerDto = JsonSerializer.Deserialize<CustomerDto>(json, _serializerOptions);
                    _logger.LogDebug("Loaded profile for {FirstName} {LastName}",
                        customerDto?.CustomerFirstName, customerDto?.CustomerLastName);

                    return _mapper.Map<Customer>(customerDto);
                }
                else
                {
                    _logger.LogWarning("GetProfileAsync failed with status {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetProfileAsync for CustomerId {CustomerId}", customerId);
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateProfileAsync(Customer customer)
        {
            Uri uri = new Uri($"{Constants.BaseUrl}/customer/update");

            try
            {
                var updateDto = _mapper.Map<UpdateCustomerDto>(customer);
                _logger.LogDebug("Updating profile for CustomerId {CustomerId}: {FirstName} {LastName}",
                    customer.Id, updateDto.FirstName, updateDto.LastName);

                string json = JsonSerializer.Serialize(updateDto, _serializerOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Profile updated successfully for CustomerId {CustomerId}", customer.Id);
                    return true;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("UpdateProfileAsync failed with status {StatusCode}: {ErrorContent}",
                        response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UpdateProfileAsync for CustomerId {CustomerId}", customer.Id);
            }

            return false;
        }
    }
}
