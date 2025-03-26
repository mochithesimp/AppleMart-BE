using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Text.Json;

namespace iPhoneBE.Service.Services
{
    public class PayPalService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _mode;
        private readonly HttpClient _httpClient;

        public PayPalService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _clientId = configuration["PayPal:ClientId"];
            _clientSecret = configuration["PayPal:ClientSecret"];
            _mode = configuration["PayPal:Mode"];
            _httpClient = httpClientFactory.CreateClient();
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var token = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);

            var tokenResponse = await _httpClient.PostAsync(
                $"{(_mode == "sandbox" ? "https://api.sandbox.paypal.com" : "https://api.paypal.com")}/v1/oauth2/token",
                new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") })
            );

            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to obtain PayPal access token");
            }

            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            return JObject.Parse(tokenContent)["access_token"]?.ToString();
        }

        public async Task<Refund> ProcessRefundAsync(string captureId, decimal amount, string currency = "USD")
        {
            try
            {
                Console.WriteLine($"Starting PayPal refund process for capture ID: {captureId}, amount: {amount} {currency}");

                if (string.IsNullOrEmpty(captureId))
                {
                    throw new ArgumentException("Capture ID cannot be empty");
                }

                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new Exception("Failed to get PayPal access token");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var captureResponse = await _httpClient.GetAsync(
                    $"{(_mode == "sandbox" ? "https://api.sandbox.paypal.com" : "https://api.paypal.com")}/v2/payments/captures/{captureId}"
                );
                var captureContent = await captureResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Capture details response: {captureContent}");

                if (!captureResponse.IsSuccessStatusCode)
                {
                    var errorDetails = JObject.Parse(captureContent);
                    var errorMessage = errorDetails["message"]?.ToString() ?? "Unknown error";
                    var errorDetailsList = errorDetails["details"]?.ToObject<List<object>>();
                    var detailsMessage = errorDetailsList != null ? string.Join(", ", errorDetailsList) : "";

                    throw new Exception($"Capture not found or invalid. Status: {captureResponse.StatusCode}, Message: {errorMessage}, Details: {detailsMessage}");
                }

                var refundRequest = new
                {
                    amount = new
                    {
                        value = amount.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                        currency_code = currency
                    },
                    note_to_payer = "Refund for order"
                };

                Console.WriteLine($"Sending refund request to PayPal: {JsonSerializer.Serialize(refundRequest)}");

                var response = await _httpClient.PostAsJsonAsync(
                    $"{(_mode == "sandbox" ? "https://api.sandbox.paypal.com" : "https://api.paypal.com")}/v2/payments/captures/{captureId}/refund",
                    refundRequest
                );

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"PayPal response: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorDetails = JObject.Parse(responseContent);
                    var errorMessage = errorDetails["message"]?.ToString() ?? "Unknown error";
                    var errorDetailsList = errorDetails["details"]?.ToObject<List<object>>();
                    var detailsMessage = errorDetailsList != null ? string.Join(", ", errorDetailsList) : "";

                    throw new Exception($"PayPal refund failed. Status: {response.StatusCode}, Message: {errorMessage}, Details: {detailsMessage}");
                }

                if (string.IsNullOrEmpty(responseContent))
                {
                    throw new Exception("Received empty response from PayPal API for refund request");
                }

                var refundData = JObject.Parse(responseContent);
                return new Refund
                {
                    id = refundData["id"]?.ToString(),
                    amount = new Amount
                    {
                        total = refundData["amount"]?["value"]?.ToString(),
                        currency = refundData["amount"]?["currency_code"]?.ToString()
                    },
                    capture_id = refundData["capture_id"]?.ToString()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PayPal refund error: {ex.Message}");
                throw new Exception($"PayPal refund failed: {ex.Message}", ex);
            }
        }

        public async Task<Payment> GetPaymentDetailsAsync(string paymentId)
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(accessToken))
                {
                    throw new Exception("Failed to get PayPal access token");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.GetAsync(
                    $"{(_mode == "sandbox" ? "https://api.sandbox.paypal.com" : "https://api.paypal.com")}/v1/payments/payment/{paymentId}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to get payment details: {errorContent}");
                }

                var paymentContent = await response.Content.ReadAsStringAsync();
                var paymentData = JObject.Parse(paymentContent);

                return new Payment
                {
                    id = paymentData["id"]?.ToString(),
                    intent = paymentData["intent"]?.ToString(),
                    state = paymentData["state"]?.ToString(),
                    transactions = paymentData["transactions"]?.ToObject<List<Transaction>>()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get payment details: {ex.Message}", ex);
            }
        }
    }
}