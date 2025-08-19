using Microsoft.Extensions.Options;
using PlanItNoww.Utils;
using System.Net.Http;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace PlanItNoww.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request);
        Task<PaymentVerificationResponse> VerifyPaymentAsync(string paymentId, string signature);
        Task<PaymentStatusResponse> GetPaymentStatusAsync(string paymentId);
    }

    public class PaymentRequest
    {
        public decimal amount { get; set; }
        public string currency { get; set; } = "INR";
        public string orderId { get; set; }
        public string customerEmail { get; set; }
        public string customerPhone { get; set; }
        public string description { get; set; }
        public Dictionary<string, string> metadata { get; set; } = new();
    }

    public class PaymentResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string paymentId { get; set; }
        public string orderId { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string gateway { get; set; }
        public Dictionary<string, object> gatewayResponse { get; set; } = new();
    }

    public class PaymentVerificationResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string paymentId { get; set; }
        public string orderId { get; set; }
        public decimal amount { get; set; }
        public string status { get; set; }
        public string gateway { get; set; }
    }

    public class PaymentStatusResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public DateTime paymentDate { get; set; }
    }

    public class PaymentService : IPaymentService
    {
        private readonly ApplicationEnvironment _config;
        private readonly ILogger<PaymentService> _logger;
        private readonly HttpClient _httpClient;

        public PaymentService(IOptions<ApplicationEnvironment> config, ILogger<PaymentService> logger, HttpClient httpClient)
        {
            _config = config.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(PaymentRequest request)
        {
            try
            {
                var gateway = _config.payment.defaultGateway.ToLower();
                
                switch (gateway)
                {
                    case "razorpay":
                        return await CreateRazorpayPaymentAsync(request);
                    case "stripe":
                        return await CreateStripePaymentAsync(request);
                    case "paytm":
                        return await CreatePaytmPaymentAsync(request);
                    default:
                        return new PaymentResponse 
                        { 
                            success = false, 
                            message = $"Unsupported payment gateway: {gateway}" 
                        };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create payment for order {OrderId}", request.orderId);
                return new PaymentResponse 
                { 
                    success = false, 
                    message = "Payment creation failed: " + ex.Message 
                };
            }
        }

        public async Task<PaymentVerificationResponse> VerifyPaymentAsync(string paymentId, string signature)
        {
            try
            {
                var gateway = _config.payment.defaultGateway.ToLower();
                
                switch (gateway)
                {
                    case "razorpay":
                        return await VerifyRazorpayPaymentAsync(paymentId, signature);
                    case "stripe":
                        return await VerifyStripePaymentAsync(paymentId);
                    case "paytm":
                        return await VerifyPaytmPaymentAsync(paymentId);
                    default:
                        return new PaymentVerificationResponse 
                        { 
                            success = false, 
                            message = $"Unsupported payment gateway: {gateway}" 
                        };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify payment {PaymentId}", paymentId);
                return new PaymentVerificationResponse 
                { 
                    success = false, 
                    message = "Payment verification failed: " + ex.Message 
                };
            }
        }

        public async Task<PaymentStatusResponse> GetPaymentStatusAsync(string paymentId)
        {
            try
            {
                var gateway = _config.payment.defaultGateway.ToLower();
                
                switch (gateway)
                {
                    case "razorpay":
                        return await GetRazorpayPaymentStatusAsync(paymentId);
                    case "stripe":
                        return await GetStripePaymentStatusAsync(paymentId);
                    case "paytm":
                        return await GetPaytmPaymentStatusAsync(paymentId);
                    default:
                        return new PaymentStatusResponse 
                        { 
                            success = false, 
                            message = $"Unsupported payment gateway: {gateway}" 
                        };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payment status for {PaymentId}", paymentId);
                return new PaymentStatusResponse 
                { 
                    success = false, 
                    message = "Failed to get payment status: " + ex.Message 
                };
            }
        }

        #region Razorpay Implementation
        private async Task<PaymentResponse> CreateRazorpayPaymentAsync(PaymentRequest request)
        {
            try
            {
                var razorpayConfig = _config.payment.razorpay;
                
                var orderData = new
                {
                    amount = (int)(request.amount * 100), // Razorpay expects amount in paise
                    currency = request.currency,
                    receipt = request.orderId,
                    notes = new Dictionary<string, string>
                    {
                        { "description", request.description },
                        { "customer_email", request.customerEmail },
                        { "customer_phone", request.customerPhone }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(orderData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Basic authentication with Razorpay
                var authToken = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{razorpayConfig.keyId}:{razorpayConfig.keySecret}"));

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

                var response = await _httpClient.PostAsync("https://api.razorpay.com/v1/orders", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var orderResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    return new PaymentResponse
                    {
                        success = true,
                        message = "Payment order created successfully",
                        paymentId = orderResponse.GetProperty("id").GetString(),
                        orderId = request.orderId,
                        amount = request.amount,
                        currency = request.currency,
                        gateway = "razorpay",
                        gatewayResponse = new Dictionary<string, object>
                        {
                            { "order_id", orderResponse.GetProperty("id").GetString() },
                            { "amount", orderResponse.GetProperty("amount").GetInt32() },
                            { "currency", orderResponse.GetProperty("currency").GetString() }
                        }
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Razorpay order creation failed: {Error}", errorContent);
                    return new PaymentResponse 
                    { 
                        success = false, 
                        message = "Failed to create Razorpay order" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Razorpay payment");
                return new PaymentResponse 
                { 
                    success = false, 
                    message = "Razorpay payment creation failed: " + ex.Message 
                };
            }
        }

        private async Task<PaymentVerificationResponse> VerifyRazorpayPaymentAsync(string paymentId, string signature)
        {
            try
            {
                var razorpayConfig = _config.payment.razorpay;
                
                // Verify signature
                var expectedSignature = ComputeHmacSha256($"{paymentId}|{razorpayConfig.webhookSecret}", razorpayConfig.keySecret);
                
                if (signature != expectedSignature)
                {
                    return new PaymentVerificationResponse 
                    { 
                        success = false, 
                        message = "Invalid signature" 
                    };
                }

                // Get payment details
                var authToken = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{razorpayConfig.keyId}:{razorpayConfig.keySecret}"));

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

                var response = await _httpClient.GetAsync($"https://api.razorpay.com/v1/payments/{paymentId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var paymentResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    return new PaymentVerificationResponse
                    {
                        success = true,
                        message = "Payment verified successfully",
                        paymentId = paymentId,
                        orderId = paymentResponse.GetProperty("order_id").GetString(),
                        amount = paymentResponse.GetProperty("amount").GetInt32() / 100m, // Convert from paise
                        status = paymentResponse.GetProperty("status").GetString(),
                        gateway = "razorpay"
                    };
                }
                else
                {
                    return new PaymentVerificationResponse 
                    { 
                        success = false, 
                        message = "Failed to get payment details" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify Razorpay payment");
                return new PaymentVerificationResponse 
                { 
                    success = false, 
                    message = "Razorpay payment verification failed: " + ex.Message 
                };
            }
        }

        private async Task<PaymentStatusResponse> GetRazorpayPaymentStatusAsync(string paymentId)
        {
            try
            {
                var razorpayConfig = _config.payment.razorpay;
                
                var authToken = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{razorpayConfig.keyId}:{razorpayConfig.keySecret}"));

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

                var response = await _httpClient.GetAsync($"https://api.razorpay.com/v1/payments/{paymentId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var paymentResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    return new PaymentStatusResponse
                    {
                        success = true,
                        message = "Payment status retrieved successfully",
                        status = paymentResponse.GetProperty("status").GetString(),
                        amount = paymentResponse.GetProperty("amount").GetInt32() / 100m,
                        currency = paymentResponse.GetProperty("currency").GetString(),
                        paymentDate = DateTimeOffset.FromUnixTimeSeconds(
                            paymentResponse.GetProperty("created_at").GetInt64()).DateTime
                    };
                }
                else
                {
                    return new PaymentStatusResponse 
                    { 
                        success = false, 
                        message = "Failed to get payment status" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Razorpay payment status");
                return new PaymentStatusResponse 
                { 
                    success = false, 
                    message = "Failed to get Razorpay payment status: " + ex.Message 
                };
            }
        }
        #endregion

        #region Stripe Implementation
        private async Task<PaymentResponse> CreateStripePaymentAsync(PaymentRequest request)
        {
            try
            {
                var stripeConfig = _config.payment.stripe;
                
                var paymentIntentData = new
                {
                    amount = (int)(request.amount * 100), // Stripe expects amount in cents
                    currency = request.currency.ToLower(),
                    metadata = new Dictionary<string, string>
                    {
                        { "order_id", request.orderId },
                        { "customer_email", request.customerEmail },
                        { "description", request.description }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(paymentIntentData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", stripeConfig.secretKey);

                var response = await _httpClient.PostAsync("https://api.stripe.com/v1/payment_intents", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var paymentIntentResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    return new PaymentResponse
                    {
                        success = true,
                        message = "Stripe payment intent created successfully",
                        paymentId = paymentIntentResponse.GetProperty("id").GetString(),
                        orderId = request.orderId,
                        amount = request.amount,
                        currency = request.currency,
                        gateway = "stripe",
                        gatewayResponse = new Dictionary<string, object>
                        {
                            { "client_secret", paymentIntentResponse.GetProperty("client_secret").GetString() },
                            { "amount", paymentIntentResponse.GetProperty("amount").GetInt32() },
                            { "currency", paymentIntentResponse.GetProperty("currency").GetString() }
                        }
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Stripe payment intent creation failed: {Error}", errorContent);
                    return new PaymentResponse 
                    { 
                        success = false, 
                        message = "Failed to create Stripe payment intent" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Stripe payment");
                return new PaymentResponse 
                { 
                    success = false, 
                    message = "Stripe payment creation failed: " + ex.Message 
                };
            }
        }

        private async Task<PaymentVerificationResponse> VerifyStripePaymentAsync(string paymentId)
        {
            try
            {
                var stripeConfig = _config.payment.stripe;
                
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", stripeConfig.secretKey);

                var response = await _httpClient.GetAsync($"https://api.stripe.com/v1/payment_intents/{paymentId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var paymentIntentResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    return new PaymentVerificationResponse
                    {
                        success = true,
                        message = "Stripe payment verified successfully",
                        paymentId = paymentId,
                        orderId = paymentIntentResponse.GetProperty("metadata").GetProperty("order_id").GetString(),
                        amount = paymentIntentResponse.GetProperty("amount").GetInt32() / 100m,
                        status = paymentIntentResponse.GetProperty("status").GetString(),
                        gateway = "stripe"
                    };
                }
                else
                {
                    return new PaymentVerificationResponse 
                    { 
                        success = false, 
                        message = "Failed to get Stripe payment details" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify Stripe payment");
                return new PaymentVerificationResponse 
                { 
                    success = false, 
                    message = "Stripe payment verification failed: " + ex.Message 
                };
            }
        }

        private async Task<PaymentStatusResponse> GetStripePaymentStatusAsync(string paymentId)
        {
            try
            {
                var stripeConfig = _config.payment.stripe;
                
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", stripeConfig.secretKey);

                var response = await _httpClient.GetAsync($"https://api.stripe.com/v1/payment_intents/{paymentId}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var paymentIntentResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    return new PaymentStatusResponse
                    {
                        success = true,
                        message = "Stripe payment status retrieved successfully",
                        status = paymentIntentResponse.GetProperty("status").GetString(),
                        amount = paymentIntentResponse.GetProperty("amount").GetInt32() / 100m,
                        currency = paymentIntentResponse.GetProperty("currency").GetString(),
                        paymentDate = DateTimeOffset.FromUnixTimeSeconds(
                            paymentIntentResponse.GetProperty("created").GetInt64()).DateTime
                    };
                }
                else
                {
                    return new PaymentStatusResponse 
                    { 
                        success = false, 
                        message = "Failed to get Stripe payment status" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Stripe payment status");
                return new PaymentStatusResponse 
                { 
                    success = false, 
                    message = "Failed to get Stripe payment status: " + ex.Message 
                };
            }
        }
        #endregion

        #region Paytm Implementation
        private async Task<PaymentResponse> CreatePaytmPaymentAsync(PaymentRequest request)
        {
            try
            {
                var paytmConfig = _config.payment.paytm;
                
                var orderData = new
                {
                    MID = paytmConfig.merchantId,
                    ORDER_ID = request.orderId,
                    TXN_AMOUNT = request.amount.ToString("F2"),
                    CHANNEL_ID = paytmConfig.channelId,
                    WEBSITE = paytmConfig.website,
                    INDUSTRY_TYPE_ID = paytmConfig.industryType,
                    CALLBACK_URL = "https://yourdomain.com/payment/callback",
                    CUST_ID = request.customerEmail,
                    MOBILE_NO = request.customerPhone,
                    EMAIL = request.customerEmail
                };

                var jsonContent = JsonSerializer.Serialize(orderData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://securegw-stage.paytm.in/order/process", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var paytmResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    return new PaymentResponse
                    {
                        success = true,
                        message = "Paytm payment order created successfully",
                        paymentId = paytmResponse.GetProperty("TXNID").GetString(),
                        orderId = request.orderId,
                        amount = request.amount,
                        currency = request.currency,
                        gateway = "paytm",
                        gatewayResponse = new Dictionary<string, object>
                        {
                            { "txn_id", paytmResponse.GetProperty("TXNID").GetString() },
                            { "order_id", paytmResponse.GetProperty("ORDERID").GetString() },
                            { "amount", paytmResponse.GetProperty("TXN_AMOUNT").GetString() }
                        }
                    };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Paytm order creation failed: {Error}", errorContent);
                    return new PaymentResponse 
                    { 
                        success = false, 
                        message = "Failed to create Paytm payment order" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Paytm payment");
                return new PaymentResponse 
                { 
                    success = false, 
                    message = "Paytm payment creation failed: " + ex.Message 
                };
            }
        }

        private async Task<PaymentVerificationResponse> VerifyPaytmPaymentAsync(string paymentId)
        {
            try
            {
                var paytmConfig = _config.payment.paytm;
                
                var verificationData = new
                {
                    MID = paytmConfig.merchantId,
                    ORDERID = paymentId,
                    TXNID = paymentId
                };

                var jsonContent = JsonSerializer.Serialize(verificationData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://securegw-stage.paytm.in/order/status", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var paytmResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    return new PaymentVerificationResponse
                    {
                        success = true,
                        message = "Paytm payment verified successfully",
                        paymentId = paymentId,
                        orderId = paytmResponse.GetProperty("ORDERID").GetString(),
                        amount = decimal.Parse(paytmResponse.GetProperty("TXNAMOUNT").GetString()),
                        status = paytmResponse.GetProperty("STATUS").GetString(),
                        gateway = "paytm"
                    };
                }
                else
                {
                    return new PaymentVerificationResponse 
                    { 
                        success = false, 
                        message = "Failed to get Paytm payment details" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify Paytm payment");
                return new PaymentVerificationResponse 
                { 
                    success = false, 
                    message = "Paytm payment verification failed: " + ex.Message 
                };
            }
        }

        private async Task<PaymentStatusResponse> GetPaytmPaymentStatusAsync(string paymentId)
        {
            try
            {
                var paytmConfig = _config.payment.paytm;
                
                var statusData = new
                {
                    MID = paytmConfig.merchantId,
                    ORDERID = paymentId,
                    TXNID = paymentId
                };

                var jsonContent = JsonSerializer.Serialize(statusData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("https://securegw-stage.paytm.in/order/status", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var paytmResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    return new PaymentStatusResponse
                    {
                        success = true,
                        message = "Paytm payment status retrieved successfully",
                        status = paytmResponse.GetProperty("STATUS").GetString(),
                        amount = decimal.Parse(paytmResponse.GetProperty("TXNAMOUNT").GetString()),
                        currency = "INR",
                        paymentDate = DateTime.UtcNow // Paytm doesn't provide exact payment time in status API
                    };
                }
                else
                {
                    return new PaymentStatusResponse 
                    { 
                        success = false, 
                        message = "Failed to get Paytm payment status" 
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get Paytm payment status");
                return new PaymentStatusResponse 
                { 
                    success = false, 
                    message = "Failed to get Paytm payment status: " + ex.Message 
                };
            }
        }
        #endregion

        private string ComputeHmacSha256(string message, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
