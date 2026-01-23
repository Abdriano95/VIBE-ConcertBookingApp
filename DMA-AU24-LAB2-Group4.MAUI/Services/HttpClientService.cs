using System.Text.Json;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    /// <summary>
    /// Base class providing shared HTTP client configuration for all API services.
    /// </summary>
    public abstract class HttpClientService
    {
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions _serializerOptions;

        protected HttpClientService(IHttpsClientHandlerService httpsClientHandlerService)
        {
#if DEBUG
            HttpMessageHandler? handler = httpsClientHandlerService.GetPlatformMessageHandler();
            _httpClient = handler != null ? new HttpClient(handler) : new HttpClient();
#else
            _httpClient = new HttpClient();
#endif
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }
    }
}
