using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;


namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRestService _restService;
        private readonly IMapper _mapper;

        public CustomerService(IRestService restService, IMapper mapper) // Add IMapper parameter to constructor
        {
            _restService = restService;
            _mapper = mapper; // Initialize _mapper
        }

        public Task<bool> RegisterCustomerAsync(Customer customer)
        {
            // Uses PostAsync method from IRestService to register a new customer
            return _restService.PostAsync<Customer, bool>(Constants.CustomerRegisterUrl, customer); // Specify type arguments to fix CS0411
        }

        public Task<Customer?> GetProfileAsync()
        {
            // Hämtar den inloggade användarens profil
            return _restService.GetAsync<Customer>(Constants.CustomerProfileUrl);
        }

        public Task<bool> UpdateProfileAsync(Customer customer)
        {
            // Uppdaterar användarens profil med PUT-metoden
            return _restService.PutAsync(Constants.CustomerUpdateUrl, customer);
        }

        public async Task<Customer?> LoginAsync(string email, string password)
        {
            var loginDto = new LoginDto
            {
                Email = email,
                Password = password
            };

            // Send login request to server
            var customerDto = await _restService.PostAsync<LoginDto, CustomerDto>(Constants.CustomerLoginUrl, loginDto);

            // If customerDto is null, return null, otherwise map to Customer and return
            return customerDto == null ? null : _mapper.Map<Customer>(customerDto);
        }
    }

}
