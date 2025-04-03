using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using WebAPI.Models.Interfaces;

namespace WebAPI.Services
{
    public class BaseApiClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;
        protected readonly ILoggerManager _logger;

        public BaseApiClient(HttpClient httpClient, IConfiguration configuration, ILoggerManager logger)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5187/api/";
            _logger = logger;
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _logger.LogInfo($"BaseApiClient initialized with base URL: {_baseUrl}");
        }

        protected async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                _logger.LogInfo($"Making GET request to endpoint: {endpoint}");
                var response = await _httpClient.GetAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"GET request to {endpoint} failed with status {response.StatusCode}: {errorContent}");
                    throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorContent}");
                }
                var result = await response.Content.ReadFromJsonAsync<T>();
                _logger.LogInfo($"Successfully retrieved data from {endpoint}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GET request to {endpoint}: {ex.Message}");
                throw;
            }
        }

        protected async Task<T> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                _logger.LogInfo($"Making POST request to endpoint: {endpoint}");
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(endpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"POST request to {endpoint} failed with status {response.StatusCode}: {errorContent}");
                    throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorContent}");
                }
                var result = await response.Content.ReadFromJsonAsync<T>();
                _logger.LogInfo($"Successfully posted data to {endpoint}");
                return result ?? throw new InvalidOperationException($"Response from {endpoint} was null");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in POST request to {endpoint}: {ex.Message}");
                throw;
            }
        }

        protected async Task<bool> DeleteAsync(string endpoint)
        {
            try
            {
                _logger.LogInfo($"Making DELETE request to endpoint: {endpoint}");
                var response = await _httpClient.DeleteAsync(endpoint);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"DELETE request to {endpoint} failed with status {response.StatusCode}: {errorContent}");
                    throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorContent}");
                }
                _logger.LogInfo($"Successfully deleted data at {endpoint}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DELETE request to {endpoint}: {ex.Message}");
                throw;
            }
        }

        protected async Task<bool> PutAsync(string endpoint, object data)
        {
            try
            {
                _logger.LogInfo($"Making PUT request to endpoint: {endpoint}");
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(endpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"PUT request to {endpoint} failed with status {response.StatusCode}: {errorContent}");
                    throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorContent}");
                }
                _logger.LogInfo($"Successfully updated data at {endpoint}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PUT request to {endpoint}: {ex.Message}");
                throw;
            }
        }
    }
}