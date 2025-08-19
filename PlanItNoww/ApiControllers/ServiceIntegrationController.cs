using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlanItNoww.Services;
using PlanItNoww.Utils;
using System.Text.Json;

namespace PlanItNoww.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceIntegrationController : ControllerBase
    {
        private readonly ILogger<ServiceIntegrationController> _logger;
        private readonly ApplicationEnvironment _config;
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;
        private readonly IPaymentService _paymentService;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IAadhaarVerificationService _aadhaarService;
        private readonly ICachingService _cachingService;
        private readonly IFileUploadService _fileUploadService;

        public ServiceIntegrationController(
            ILogger<ServiceIntegrationController> logger,
            IOptions<ApplicationEnvironment> config,
            IEmailService emailService,
            ISMSService smsService,
            IPaymentService paymentService,
            IPushNotificationService pushNotificationService,
            IAadhaarVerificationService aadhaarService,
            ICachingService cachingService,
            IFileUploadService fileUploadService)
        {
            _logger = logger;
            _config = config.Value;
            _emailService = emailService;
            _smsService = smsService;
            _paymentService = paymentService;
            _pushNotificationService = pushNotificationService;
            _aadhaarService = aadhaarService;
            _cachingService = cachingService;
            _fileUploadService = fileUploadService;
        }

        [HttpGet("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            try
            {
                var result = await _emailService.SendOTPAsync("test@example.com", "123456", "TEST");
                return Ok(new { success = result, message = "Email service test completed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email service test failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("test-sms")]
        public async Task<IActionResult> TestSMS()
        {
            try
            {
                var result = await _smsService.SendOTPAsync("+1234567890", "123456", "TEST");
                return Ok(new { success = result, message = "SMS service test completed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SMS service test failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("test-payment")]
        public async Task<IActionResult> TestPayment()
        {
            try
            {
                var paymentRequest = new PaymentRequest
                {
                    orderId = "TEST_ORDER_001",
                    amount = 100.00m,
                    currency = "USD",
                    customerEmail = "test@example.com",
                    customerPhone = "+1234567890",
                    description = "Test payment"
                };

                var result = await _paymentService.CreatePaymentAsync(paymentRequest);
                return Ok(new { success = true, payment = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment service test failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("test-push-notification")]
        public async Task<IActionResult> TestPushNotification()
        {
            try
            {
                var notificationRequest = new PushNotificationRequest
                {
                    title = "Test Notification",
                    message = "This is a test push notification",
                    data = new Dictionary<string, object>
                    {
                        { "type", "test" },
                        { "timestamp", DateTime.UtcNow }
                    }
                };

                var result = await _pushNotificationService.SendNotificationAsync(notificationRequest);
                return Ok(new { success = result, message = "Push notification service test completed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Push notification service test failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("test-aadhaar")]
        public async Task<IActionResult> TestAadhaar()
        {
            try
            {
                var aadhaarRequest = new AadhaarVerificationRequest
                {
                    aadhaarNumber = "123456789012",
                    customerName = "Test User",
                    customerMobile = "+1234567890",
                    customerEmail = "test@example.com"
                };

                var result = await _aadhaarService.VerifyAadhaarAsync(aadhaarRequest);
                return Ok(new { success = true, verification = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Aadhaar verification service test failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("test-caching")]
        public async Task<IActionResult> TestCaching()
        {
            try
            {
                var testKey = "test_cache_key";
                var testValue = "test_cache_value";

                // Set value
                await _cachingService.SetAsync(testKey, testValue, TimeSpan.FromMinutes(5));

                // Get value
                var retrievedValue = await _cachingService.GetAsync<string>(testKey);

                // Check if exists
                var exists = await _cachingService.ExistsAsync(testKey);

                // Delete value
                await _cachingService.DeleteAsync(testKey);

                return Ok(new { 
                    success = true, 
                    message = "Caching service test completed",
                    setValue = testValue,
                    retrievedValue = retrievedValue,
                    exists = exists
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Caching service test failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("test-file-upload")]
        public async Task<IActionResult> TestFileUpload()
        {
            try
            {
                // Create a test file stream
                var testContent = "This is a test file content";
                var testBytes = System.Text.Encoding.UTF8.GetBytes(testContent);
                var testStream = new MemoryStream(testBytes);

                var uploadRequest = new FileUploadRequest
                {
                    fileStream = testStream,
                    fileName = "test-file.txt",
                    contentType = "text/plain"
                };

                var result = await _fileUploadService.UploadFileAsync(uploadRequest);
                return Ok(new { success = true, upload = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File upload service test failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("test-all-services")]
        public async Task<IActionResult> TestAllServices()
        {
            try
            {
                var results = new Dictionary<string, object>();

                // Test Email Service
                try
                {
                    var emailResult = await _emailService.SendOTPAsync("test@example.com", "123456", "TEST");
                    results["email"] = new { success = emailResult, message = "Email service working" };
                }
                catch (Exception ex)
                {
                    results["email"] = new { success = false, message = ex.Message };
                }

                // Test SMS Service
                try
                {
                    var smsResult = await _smsService.SendOTPAsync("+1234567890", "123456", "TEST");
                    results["sms"] = new { success = smsResult, message = "SMS service working" };
                }
                catch (Exception ex)
                {
                    results["sms"] = new { success = false, message = ex.Message };
                }

                // Test Caching Service
                try
                {
                    var testKey = "integration_test_key";
                    await _cachingService.SetAsync(testKey, "test_value", TimeSpan.FromMinutes(1));
                    var cacheValue = await _cachingService.GetAsync<string>(testKey);
                    await _cachingService.DeleteAsync(testKey);
                    results["caching"] = new { success = true, message = "Caching service working", value = cacheValue };
                }
                catch (Exception ex)
                {
                    results["caching"] = new { success = false, message = ex.Message };
                }

                // Test Payment Service
                try
                {
                    var paymentRequest = new PaymentRequest
                    {
                        orderId = "INTEGRATION_TEST_001",
                        amount = 50.00m,
                        currency = "USD",
                        customerEmail = "test@example.com",
                        customerPhone = "+1234567890",
                        description = "Integration test payment"
                    };
                    var paymentResult = await _paymentService.CreatePaymentAsync(paymentRequest);
                    results["payment"] = new { success = true, message = "Payment service working", payment = paymentResult };
                }
                catch (Exception ex)
                {
                    results["payment"] = new { success = false, message = ex.Message };
                }

                // Test Push Notification Service
                try
                {
                    var notificationRequest = new PushNotificationRequest
                    {
                        title = "Integration Test",
                        message = "Testing push notification service",
                        data = new Dictionary<string, object> { { "test", true } }
                    };
                    var pushResult = await _pushNotificationService.SendNotificationAsync(notificationRequest);
                    results["pushNotification"] = new { success = pushResult, message = "Push notification service working" };
                }
                catch (Exception ex)
                {
                    results["pushNotification"] = new { success = false, message = ex.Message };
                }

                // Test Aadhaar Service
                try
                {
                    var aadhaarRequest = new AadhaarVerificationRequest
                    {
                        aadhaarNumber = "123456789012",
                        customerName = "Integration Test User",
                        customerMobile = "+1234567890",
                        customerEmail = "test@example.com"
                    };
                    var aadhaarResult = await _aadhaarService.VerifyAadhaarAsync(aadhaarRequest);
                    results["aadhaar"] = new { success = true, message = "Aadhaar service working", verification = aadhaarResult };
                }
                catch (Exception ex)
                {
                    results["aadhaar"] = new { success = false, message = ex.Message };
                }

                return Ok(new { 
                    success = true, 
                    message = "All services integration test completed",
                    timestamp = DateTime.UtcNow,
                    results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "All services integration test failed");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("configuration-status")]
        public IActionResult GetConfigurationStatus()
        {
            try
            {
                var status = new Dictionary<string, object>();

                // Check Email Configuration
                status["email"] = new
                {
                    provider = _config.email?.provider,
                    smtpConfigured = !string.IsNullOrEmpty(_config.email?.smtp?.host),
                    sendgridConfigured = !string.IsNullOrEmpty(_config.email?.sendgrid?.apiKey)
                };

                // Check SMS Configuration
                status["sms"] = new
                {
                    provider = _config.sms?.provider,
                    twilioConfigured = !string.IsNullOrEmpty(_config.sms?.twilio?.accountSid),
                    msg91Configured = !string.IsNullOrEmpty(_config.sms?.msg91?.authKey)
                };

                // Check Payment Configuration
                status["payment"] = new
                {
                    defaultGateway = _config.payment?.defaultGateway,
                    razorpayConfigured = !string.IsNullOrEmpty(_config.payment?.razorpay?.keyId),
                    stripeConfigured = !string.IsNullOrEmpty(_config.payment?.stripe?.publishableKey),
                    paytmConfigured = !string.IsNullOrEmpty(_config.payment?.paytm?.merchantId)
                };

                // Check Push Notification Configuration
                status["pushNotification"] = new
                {
                    firebaseConfigured = !string.IsNullOrEmpty(_config.pushNotification?.firebase?.serverKey),
                    oneSignalConfigured = !string.IsNullOrEmpty(_config.pushNotification?.onesignal?.appId)
                };

                // Check Aadhaar Configuration
                status["aadhaar"] = new
                {
                    apiKeyConfigured = !string.IsNullOrEmpty(_config.aadhaar?.apiKey),
                    baseUrlConfigured = !string.IsNullOrEmpty(_config.aadhaar?.baseUrl)
                };

                // Check Caching Configuration
                status["caching"] = new
                {
                    redisConfigured = !string.IsNullOrEmpty(_config.caching?.redis?.connectionString)
                };

                // Check AWS S3 Configuration
                status["awsS3"] = new
                {
                    enabled = _config.awss3config?.iss3enabled ?? false,
                    endpointConfigured = !string.IsNullOrEmpty(_config.awss3config?.endpoint),
                    bucketConfigured = !string.IsNullOrEmpty(_config.awss3config?.bucketname)
                };

                return Ok(new { 
                    success = true, 
                    message = "Configuration status retrieved",
                    timestamp = DateTime.UtcNow,
                    status = status
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get configuration status");
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
