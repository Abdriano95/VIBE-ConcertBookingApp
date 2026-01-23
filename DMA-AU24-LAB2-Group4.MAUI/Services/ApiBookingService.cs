using AutoMapper;
using CommunityToolkit.Maui.Core.Extensions;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    /// <summary>
    /// Service for booking-related API operations.
    /// </summary>
    public class ApiBookingService : HttpClientService, IApiBookingService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ApiBookingService> _logger;

        public ApiBookingService(
            IHttpsClientHandlerService httpsClientHandlerService,
            IMapper mapper,
            ILogger<ApiBookingService> logger) : base(httpsClientHandlerService)
        {
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ObservableCollection<Booking>?> GetAllBookingsAsync()
        {
            var bookings = new ObservableCollection<Booking>();
            Uri uri = new Uri(string.Format(Constants.BookingUrl, string.Empty));

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    bookings = _mapper.Map<List<Booking>>(
                        JsonSerializer.Deserialize<List<BookingDto>>(content, _serializerOptions)
                    ).ToObservableCollection();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all bookings");
            }

            return bookings;
        }

        /// <inheritdoc />
        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            Uri uri = new Uri($"{Constants.BaseUrl}/booking/{bookingId}");

            try
            {
                _logger.LogDebug("Fetching booking details for BookingId {BookingId}", bookingId);

                HttpResponseMessage response = await _httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    _logger.LogDebug("Received booking JSON response for BookingId {BookingId}", bookingId);

                    var bookingDto = JsonSerializer.Deserialize<BookingDto>(json, _serializerOptions);
                    if (bookingDto != null)
                    {
                        _logger.LogDebug("Deserialized BookingDto: {ConcertTitle}, {PerformanceDate}",
                            bookingDto.ConcertTitle, bookingDto.PerformanceDate);
                        return _mapper.Map<Booking>(bookingDto);
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("GetBookingByIdAsync failed with status {StatusCode}: {ErrorContent}",
                        response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetBookingByIdAsync for BookingId {BookingId}", bookingId);
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingsByCustomerIdAsync(int customerId)
        {
            Uri uri = new Uri($"{Constants.BaseUrl}/booking/customer/{customerId}");

            try
            {
                var response = await _httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var bookingDtos = await response.Content.ReadFromJsonAsync<IEnumerable<BookingDto>>();
                    return _mapper.Map<IEnumerable<Booking>>(bookingDtos);
                }

                _logger.LogWarning("GetBookingsByCustomerIdAsync failed with status {StatusCode}", response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetBookingsByCustomerIdAsync for CustomerId {CustomerId}", customerId);
            }

            return Enumerable.Empty<Booking>();
        }

        /// <inheritdoc />
        public async Task<bool> CreateBookingAsync(BookingCreateDto bookingDto)
        {
            Uri uri = new Uri($"{Constants.BaseUrl}/booking");

            try
            {
                string json = JsonSerializer.Serialize(bookingDto, _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Booking successfully created");
                    return true;
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("CreateBookingAsync failed with status {StatusCode}: {ErrorContent}",
                        response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in CreateBookingAsync");
            }

            return false;
        }

        /// <inheritdoc />
        public async Task SaveBookingAsync(Booking booking, bool isNewBooking = false)
        {
            Uri uri = new Uri(string.Format(Constants.BookingUrl, string.Empty));

            try
            {
                string json = JsonSerializer.Serialize<BookingDto>(_mapper.Map<BookingDto>(booking), _serializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (isNewBooking)
                    response = await _httpClient.PostAsync(uri, content);
                else
                    response = await _httpClient.PutAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    _logger.LogInformation("Booking successfully saved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save booking");
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteBookingAsync(int bookingId)
        {
            Uri uri = new Uri($"{Constants.BaseUrl}/booking/{bookingId}");

            try
            {
                var response = await _httpClient.DeleteAsync(uri);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in DeleteBookingAsync for BookingId {BookingId}", bookingId);
                return false;
            }
        }
    }
}
