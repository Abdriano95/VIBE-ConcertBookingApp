using DMA_AU24_LAB2_Group4.MAUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    public class CustomerService : ICustomerService
    {
        IRestService _restService;

        public CustomerService(IRestService service)
        {
            _restService = service;
        }

        public Task<bool> RegisterCustomerAsync(Customer customer)
        {
            return _restService.RegisterCustomerAsync(customer);
        }

        public Task<Customer?> LoginAsync(string email, string password)
        {
            return _restService.LoginAsync(email, password);
        }
    }
}
