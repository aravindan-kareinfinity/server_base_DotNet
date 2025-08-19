using Microsoft.Extensions.Options;
using PlanItNoww.Utils;
using System.Net.Http;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace PlanItNoww.Services
{
    public interface IAadhaarVerificationService
    {
        Task<AadhaarVerificationResponse> VerifyAadhaarAsync(AadhaarVerificationRequest request);
        Task<AadhaarVerificationResponse> GetVerificationStatusAsync(string verificationId);
        Task<bool> IsAadhaarValidAsync(string aadhaarNumber);
    }

    public class AadhaarVerificationRequest
    {
        public string aadhaarNumber { get; set; }
        public string customerName { get; set; }
        public string customerMobile { get; set; }
        public string customerEmail { get; set; }
        public string purpose { get; set; } = "KYC_VERIFICATION";
        public Dictionary<string, string> metadata { get; set; } = new();
    }

    public class AadhaarVerificationResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string verificationId { get; set; }
        public string aadhaarNumber { get; set; }
        public string customerName { get; set; }
        public string verificationStatus { get; set; }
        public string verificationSource { get; set; }
        public DateTime verificationDate { get; set; }
        public Dictionary<string, object> responseData { get; set; } = new();
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
    }

    public class AadhaarVerificationService : IAadhaarVerificationService
    {
        private readonly ApplicationEnvironment _config;
        private readonly ILogger<AadhaarVerificationService> _logger;
        private readonly HttpClient _httpClient;

        public AadhaarVerificationService(IOptions<ApplicationEnvironment> config, ILogger<AadhaarVerificationService> logger, HttpClient httpClient)
        {
            _config = config.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<AadhaarVerificationResponse> VerifyAadhaarAsync(AadhaarVerificationRequest request)
        {
            try
            {
                // Validate Aadhaar number format
                if (!IsValidAadhaarFormat(request.aadhaarNumber))
                {
                    return new AadhaarVerificationResponse
                    {
                        success = false,
                        message = "Invalid Aadhaar number format",
                        errorCode = "INVALID_FORMAT",
                        errorMessage = "Aadhaar number must be 12 digits"
                    };
                }

                // Mask Aadhaar number for security
                var maskedAadhaar = MaskAadhaarNumber(request.aadhaarNumber);

                // Create verification request
                var verificationData = new
                {
                    aadhaar_number = request.aadhaarNumber,
                    customer_name = request.customerName,
                    customer_mobile = request.customerMobile,
                    customer_email = request.customerEmail,
                    purpose = request.purpose,
                    merchant_id = _config.aadhaar.merchantId,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    metadata = request.metadata
                };

                var jsonContent = JsonSerializer.Serialize(verificationData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Add authentication headers
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                var signature = GenerateSignature(verificationData, timestamp);

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _config.aadhaar.apiKey);
                _httpClient.DefaultRequestHeaders.Add("X-Timestamp", timestamp);
                _httpClient.DefaultRequestHeaders.Add("X-Signature", signature);
                _httpClient.DefaultRequestHeaders.Add("X-Merchant-ID", _config.aadhaar.merchantId);

                var baseUrl = _config.aadhaar.isProduction 
                    ? "https://api.aadhaar.uidai.gov.in" 
                    : "https://testapi.aadhaar.uidai.gov.in";

                var response = await _httpClient.PostAsync($"{baseUrl}/v1/kyc/verify", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    var verificationResponse = new AadhaarVerificationResponse
                    {
                        success = true,
                        message = "Aadhaar verification initiated successfully",
                        verificationId = apiResponse.GetProperty("verification_id").GetString(),
                        aadhaarNumber = maskedAadhaar,
                        customerName = request.customerName,
                        verificationStatus = "PENDING",
                        verificationSource = "API",
                        verificationDate = DateTime.UtcNow,
                        responseData = new Dictionary<string, object>
                        {
                            { "api_response", responseContent },
                            { "verification_id", apiResponse.GetProperty("verification_id").GetString() }
                        }
                    };

                    _logger.LogInformation("Aadhaar verification initiated for {AadhaarNumber}", maskedAadhaar);
                    return verificationResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Aadhaar verification failed with status: {StatusCode}, Error: {Error}", 
                        response.StatusCode, errorContent);

                    return new AadhaarVerificationResponse
                    {
                        success = false,
                        message = "Aadhaar verification failed",
                        errorCode = response.StatusCode.ToString(),
                        errorMessage = errorContent,
                        aadhaarNumber = maskedAadhaar,
                        customerName = request.customerName,
                        verificationStatus = "FAILED",
                        verificationSource = "API",
                        verificationDate = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initiate Aadhaar verification for {AadhaarNumber}", 
                    MaskAadhaarNumber(request.aadhaarNumber));

                return new AadhaarVerificationResponse
                {
                    success = false,
                    message = "Aadhaar verification failed: " + ex.Message,
                    errorCode = "EXCEPTION",
                    errorMessage = ex.Message,
                    aadhaarNumber = MaskAadhaarNumber(request.aadhaarNumber),
                    customerName = request.customerName,
                    verificationStatus = "FAILED",
                    verificationSource = "API",
                    verificationDate = DateTime.UtcNow
                };
            }
        }

        public async Task<AadhaarVerificationResponse> GetVerificationStatusAsync(string verificationId)
        {
            try
            {
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                var signature = GenerateSignature(new { verification_id = verificationId }, timestamp);

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _config.aadhaar.apiKey);
                _httpClient.DefaultRequestHeaders.Add("X-Timestamp", timestamp);
                _httpClient.DefaultRequestHeaders.Add("X-Signature", signature);
                _httpClient.DefaultRequestHeaders.Add("X-Merchant-ID", _config.aadhaar.merchantId);

                var baseUrl = _config.aadhaar.isProduction 
                    ? "https://api.aadhaar.uidai.gov.in" 
                    : "https://testapi.aadhaar.uidai.gov.in";

                var response = await _httpClient.GetAsync($"{baseUrl}/v1/kyc/status/{verificationId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    var verificationResponse = new AadhaarVerificationResponse
                    {
                        success = true,
                        message = "Verification status retrieved successfully",
                        verificationId = verificationId,
                        verificationStatus = apiResponse.GetProperty("status").GetString(),
                        verificationSource = "API",
                        verificationDate = DateTime.UtcNow,
                        responseData = new Dictionary<string, object>
                        {
                            { "api_response", responseContent },
                            { "status", apiResponse.GetProperty("status").GetString() }
                        }
                    };

                    // Extract additional information if available
                    if (apiResponse.TryGetProperty("aadhaar_number", out var aadhaarNumber))
                    {
                        verificationResponse.aadhaarNumber = MaskAadhaarNumber(aadhaarNumber.GetString());
                    }

                    if (apiResponse.TryGetProperty("customer_name", out var customerName))
                    {
                        verificationResponse.customerName = customerName.GetString();
                    }

                    _logger.LogInformation("Aadhaar verification status retrieved for {VerificationId}: {Status}", 
                        verificationId, verificationResponse.verificationStatus);

                    return verificationResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to get verification status with status: {StatusCode}, Error: {Error}", 
                        response.StatusCode, errorContent);

                    return new AadhaarVerificationResponse
                    {
                        success = false,
                        message = "Failed to get verification status",
                        errorCode = response.StatusCode.ToString(),
                        errorMessage = errorContent,
                        verificationId = verificationId,
                        verificationStatus = "UNKNOWN",
                        verificationSource = "API",
                        verificationDate = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get verification status for {VerificationId}", verificationId);

                return new AadhaarVerificationResponse
                {
                    success = false,
                    message = "Failed to get verification status: " + ex.Message,
                    errorCode = "EXCEPTION",
                    errorMessage = ex.Message,
                    verificationId = verificationId,
                    verificationStatus = "UNKNOWN",
                    verificationSource = "API",
                    verificationDate = DateTime.UtcNow
                };
            }
        }

        public async Task<bool> IsAadhaarValidAsync(string aadhaarNumber)
        {
            try
            {
                // Basic format validation
                if (!IsValidAadhaarFormat(aadhaarNumber))
                {
                    return false;
                }

                // TODO: Implement actual Aadhaar validation logic
                // This could involve checking against a database of valid Aadhaar numbers
                // or using UIDAI's validation APIs

                // For now, return true if format is valid
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate Aadhaar number {AadhaarNumber}", 
                    MaskAadhaarNumber(aadhaarNumber));
                return false;
            }
        }

        #region Helper Methods
        private bool IsValidAadhaarFormat(string aadhaarNumber)
        {
            if (string.IsNullOrEmpty(aadhaarNumber))
                return false;

            // Remove any non-digit characters
            var digitsOnly = new string(aadhaarNumber.Where(char.IsDigit).ToArray());

            // Aadhaar must be exactly 12 digits
            if (digitsOnly.Length != 12)
                return false;

            // Check if it's not all zeros
            if (digitsOnly.All(c => c == '0'))
                return false;

            // Basic checksum validation (Verhoeff algorithm)
            return ValidateAadhaarChecksum(digitsOnly);
        }

        private bool ValidateAadhaarChecksum(string aadhaarNumber)
        {
            try
            {
                // Simple validation: Check if the number doesn't start with 0 or 1
                // and doesn't contain all same digits
                if (aadhaarNumber[0] == '0' || aadhaarNumber[0] == '1')
                    return false;

                // Check if all digits are not the same
                if (aadhaarNumber.All(c => c == aadhaarNumber[0]))
                    return false;

                // TODO: Implement proper Verhoeff algorithm for checksum validation
                // For now, return true for basic validation
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string MaskAadhaarNumber(string aadhaarNumber)
        {
            if (string.IsNullOrEmpty(aadhaarNumber) || aadhaarNumber.Length < 8)
                return aadhaarNumber;

            var digitsOnly = new string(aadhaarNumber.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length != 12)
                return aadhaarNumber;

            // Mask middle 8 digits: XXXX-XXXX-XXXX
            return $"{digitsOnly.Substring(0, 4)}-XXXX-{digitsOnly.Substring(8, 4)}";
        }

        private string GenerateSignature(object data, string timestamp)
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(data);
                var message = $"{jsonString}{timestamp}{_config.aadhaar.apiSecret}";

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_config.aadhaar.apiSecret));
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                return Convert.ToHexString(hash).ToLower();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate signature");
                return string.Empty;
            }
        }
        #endregion
    }
}
