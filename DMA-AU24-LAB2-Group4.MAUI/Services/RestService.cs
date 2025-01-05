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
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly IHttpsClientHandlerService _httpsClientHandlerService;
        private readonly IMapper _mapper;

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

        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>(_serializerOptions);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GET: {ex.Message}");
            }
            return default;
        }

        public async Task<IEnumerable<T>?> GetAllAsync<T>(string url)
        {
            return await GetAsync<IEnumerable<T>>(url);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            try
            {
                var response = await _client.PostAsJsonAsync(url, data, _serializerOptions);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TResponse>(_serializerOptions);
                }
                Debug.WriteLine($"POST failed with status code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in POST: {ex.Message}");
            }
            return default;
        }

        public async Task<bool> PutAsync<T>(string url, T data)
        {
            try
            {
                // User _mapper if needed
                var mappedData = _mapper.Map<T>(data);
                var response = await _client.PutAsJsonAsync(url, mappedData, _serializerOptions);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in PUT: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string url)
        {
            try
            {
                var response = await _client.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in DELETE: {ex.Message}");
                return false;
            }
        }
    }
}
