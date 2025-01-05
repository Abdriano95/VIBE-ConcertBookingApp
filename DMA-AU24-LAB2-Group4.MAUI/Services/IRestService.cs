using DMA_AU24_LAB2_Group4.MAUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMA_AU24_LAB2_Group4.MAUI.Services
{
    public interface IRestService
    {
        Task<T?> GetAsync<T>(string url);
        Task<IEnumerable<T>?> GetAllAsync<T>(string url);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data);
        Task<bool> PutAsync<T>(string url, T data);
        Task<bool> DeleteAsync(string url);
    }
}
