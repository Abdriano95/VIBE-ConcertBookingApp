using AutoMapper;
using CommunityToolkit.Maui.Core.Extensions;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    public class RestService : IRestService
    {
        private HttpClient _client;
        private JsonSerializerOptions _serializerOptions;
        private IHttpsClientHandlerService _httpsClientHandlerService;
        private IMapper _mapper;
        public ObservableCollection<Booking>? Items { get; set; }
        public RestService(IHttpsClientHandlerService service, IMapper mapper)
        {
            _mapper = mapper;
#if DEBUG
            _httpsClientHandlerService = service;
            HttpMessageHandler handler = _httpsClientHandlerService.GetPlatformMessageHandler();
            if (handler != null)
                _client = new HttpClient(handler);
            else
                _client = new HttpClient();
#else
_client = new HttpClient();
#endif
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }
        public async Task<ObservableCollection<Booking>?> RefreshDataAsync()
        {
            Items = new ObservableCollection<Booking>();
            Uri uri = new Uri(string.Format(Constants.BookingUrl, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Items = _mapper.Map<List<Booking>>
                    (
                    JsonSerializer.Deserialize<List<BookingDto>>(content, _serializerOptions)
                    ).ToObservableCollection();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
            return Items;
        }
        public async Task SaveBookingAsync(Booking booking, bool isNewBooking = false)
        {
            Uri uri = new Uri(string.Format(Constants.BookingUrl, string.Empty));
            try
            {
                string json = JsonSerializer.Serialize<BookingDto>(_mapper.Map<BookingDto>(booking),
                _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null!;
                if (isNewBooking)
                    response = await _client.PostAsync(uri, content);
                else
                    response = await _client.PutAsync(uri, content);
                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tBooking successfully saved.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }
        public async Task DeleteBookingAsync(int id) // bytade från string id till int id, kan behövas att se över
        {
            Uri uri = new Uri(string.Format(Constants.BookingUrl, id));
            try
            {
                HttpResponseMessage response = await _client.DeleteAsync(uri);
                if (response.IsSuccessStatusCode)
                    Debug.WriteLine(@"\tBooking successfully deleted.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }
        // CUSTOMER
        public async Task<bool> RegisterCustomerAsync(Customer customer)
        {
            try
            {
                var dto = _mapper.Map<RegisterCustomerDto>(customer);
                Uri uri = new Uri(Constants.CustomerRegisterUrl);
                var response = await _client.PostAsJsonAsync(uri, dto);
                if (!response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Registration failed. StatusCode: {response.StatusCode}");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Error Content: {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return false;
            }
        }

        public async Task<Customer?> LoginAsync(string email, string password)
        {
            try
            {
                var loginDto = new LoginDto { Email = email, Password = password };
                Uri uri = new Uri(Constants.CustomerLoginUrl);
                var response = await _client.PostAsJsonAsync(uri, loginDto);

                if (!response.IsSuccessStatusCode) return null;

                var customerDto = await response.Content.ReadFromJsonAsync<CustomerDto>(_serializerOptions);

                if (customerDto == null)
                {
                    Debug.WriteLine("Failed to deserialize CustomerDto");
                    return null;
                }
                return _mapper.Map<Customer>(customerDto);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                return null;
            }
        }
    }
}