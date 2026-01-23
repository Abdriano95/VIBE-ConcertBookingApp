using AutoMapper;
using CommunityToolkit.Maui.Core.Extensions;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.MAUI.Models;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    /// <summary>
    /// Service for concert-related API operations.
    /// </summary>
    public class ApiConcertService : HttpClientService, IApiConcertService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ApiConcertService> _logger;

        public ApiConcertService(
            IHttpsClientHandlerService httpsClientHandlerService,
            IMapper mapper,
            ILogger<ApiConcertService> logger) : base(httpsClientHandlerService)
        {
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<ObservableCollection<Concert>?> GetAllConcertsAsync()
        {
            var concerts = new ObservableCollection<Concert>();
            Uri uri = new Uri(string.Format(Constants.ConcertUrl, string.Empty));

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var concertDtos = JsonSerializer.Deserialize<List<ConcertDto>>(content, _serializerOptions);
                    concerts = _mapper.Map<List<Concert>>(concertDtos).ToObservableCollection();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all concerts");
            }

            return concerts;
        }

        /// <inheritdoc />
        public async Task<Concert?> GetConcertByIdAsync(int concertId)
        {
            Uri uri = new Uri(string.Format(Constants.ConcertUrl, concertId));

            try
            {
                var response = await _httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var concertDto = JsonSerializer.Deserialize<ConcertDto>(content, _serializerOptions);
                    if (concertDto != null)
                    {
                        return _mapper.Map<Concert>(concertDto);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get concert by ID {ConcertId}", concertId);
            }

            return null;
        }
    }
}
