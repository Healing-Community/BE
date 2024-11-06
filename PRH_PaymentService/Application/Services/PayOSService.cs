using Application.Interfaces.Services;
using Domain.Contracts;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Application.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly HttpClient _httpClient;
        private readonly string _checksumKey;

        public PayOSService(HttpClient httpClient, IConfiguration configuration)
        {
            var payOsConfig = configuration.GetSection("PayOS");
            var clientId = payOsConfig["ClientId"];
            var apiKey = payOsConfig["ApiKey"];
            _checksumKey = payOsConfig["ChecksumKey"];

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new InvalidOperationException("Client ID is not configured.");
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("API key is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_checksumKey))
            {
                throw new InvalidOperationException("Checksum key is not configured.");
            }

            _httpClient = httpClient;
        }

        private string ComputeChecksum(string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hashBytes).ToLower();
        }

        public async Task<CreatePaymentResponse> CreatePaymentLink(PaymentRequest request)
        {
            var jsonData = JsonSerializer.Serialize(request);
            var checksum = ComputeChecksum(jsonData);

            // Thêm checksum vào Header
            _httpClient.DefaultRequestHeaders.Add("Checksum", checksum);

            var apiUrl = "https://api.payos.com/api/payments/initiate"; // Thay bằng URL thực tế
            var response = await _httpClient.PostAsJsonAsync(apiUrl, request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CreatePaymentResponse>();

            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize the payment response.");
            }

            return result;
        }

        // Triển khai các phương thức khác nếu cần
    }
}
