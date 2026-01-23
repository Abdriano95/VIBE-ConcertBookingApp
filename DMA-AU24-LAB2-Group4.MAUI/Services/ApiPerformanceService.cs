using AutoMapper;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    /// <summary>
    /// Service for performance-related API operations.
    /// </summary>
    public class ApiPerformanceService : HttpClientService, IApiPerformanceService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ApiPerformanceService> _logger;

        public ApiPerformanceService(
            IHttpsClientHandlerService httpsClientHandlerService,
            IMapper mapper,
            ILogger<ApiPerformanceService> logger) : base(httpsClientHandlerService)
        {
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ObservableCollection<Performance>> GetAvailablePerformancesAsync(int concertId, int customerId)
        {
            var performances = new ObservableCollection<Performance>();
            Uri uri = new Uri($"{Constants.BaseUrl}/performance/available/{concertId}/{customerId}");

            try
            {
                _logger.LogDebug("Fetching available performances for ConcertId {ConcertId}, CustomerId {CustomerId}",
                    concertId, customerId);

                HttpResponseMessage response = await _httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    var performanceDtos = JsonSerializer.Deserialize<List<PerformanceDto>>(jsonContent, _serializerOptions);
                    performances = new ObservableCollection<Performance>(_mapper.Map<List<Performance>>(performanceDtos));
                    _logger.LogDebug("Loaded {Count} available performances for ConcertId {ConcertId}",
                        performances.Count, concertId);
                }
                else
                {
                    _logger.LogWarning("GetAvailablePerformancesAsync failed with status {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in GetAvailablePerformancesAsync for ConcertId {ConcertId}", concertId);
            }

            return performances;
        }
    }
}
