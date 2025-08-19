using Microsoft.Extensions.Options;
using PlanItNoww.Utils;
using System.Net.Http;
using System.Text.Json;

namespace PlanItNoww.Services
{
    public interface ISMSService
    {
        Task<bool> SendSMSAsync(string to, string message);
        Task<bool> SendOTPAsync(string to, string otpCode, string purpose = "LOGIN");
    }

    public class SMSService : ISMSService
    {
        private readonly ApplicationEnvironment _config;
        private readonly ILogger<SMSService> _logger;
        private readonly HttpClient _httpClient;

        public SMSService(IOptions<ApplicationEnvironment> config, ILogger<SMSService> logger, HttpClient httpClient)
        {
            _config = config.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<bool> SendSMSAsync(string to, string message)
        {
            try
            {
                if (_config.sms.provider.ToLower() == "twilio")
                {
                    return await SendViaTwilioAsync(to, message);
                }
                else if (_config.sms.provider.ToLower() == "msg91")
                {
                    return await SendViaMSG91Async(to, message);
                }
                else
                {
                    _logger.LogWarning("Unknown SMS provider: {Provider}", _config.sms.provider);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", to);
                return false;
            }
        }

        public async Task<bool> SendOTPAsync(string to, string otpCode, string purpose = "LOGIN")
        {
            var message = $"Your OTP for {purpose} is: {otpCode}. Valid for {_config.sms.otpExpiryMinutes} minutes. Do not share with anyone.";
            return await SendSMSAsync(to, message);
        }

        private async Task<bool> SendViaTwilioAsync(string to, string message)
        {
            try
            {
                var twilioConfig = _config.sms.twilio;
                
                // Twilio REST API endpoint
                var url = $"https://api.twilio.com/2010-04-01/Accounts/{twilioConfig.accountSid}/Messages.json";
                
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("To", to),
                    new KeyValuePair<string, string>("From", twilioConfig.fromNumber),
                    new KeyValuePair<string, string>("Body", message)
                });

                // Basic authentication with Twilio
                var authToken = Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes($"{twilioConfig.accountSid}:{twilioConfig.authToken}"));

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);

                var response = await _httpClient.PostAsync(url, formData);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("SMS sent successfully via Twilio to {PhoneNumber}", to);
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Twilio SMS failed with status: {StatusCode}, Error: {Error}", 
                        response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS via Twilio to {PhoneNumber}", to);
                return false;
            }
        }

        private async Task<bool> SendViaMSG91Async(string to, string message)
        {
            try
            {
                var msg91Config = _config.sms.msg91;
                
                // MSG91 API endpoint
                var url = "https://api.msg91.com/api/v5/flow/";
                
                var requestData = new
                {
                    flow_id = "your_flow_id", // You need to create a flow in MSG91 dashboard
                    sender = msg91Config.senderId,
                    mobiles = to,
                    VAR1 = message,
                    VAR2 = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
                };

                var jsonContent = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                
                _httpClient.DefaultRequestHeaders.Add("authkey", msg91Config.authKey);

                var response = await _httpClient.PostAsync(url, content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("SMS sent successfully via MSG91 to {PhoneNumber}", to);
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("MSG91 SMS failed with status: {StatusCode}, Error: {Error}", 
                        response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS via MSG91 to {PhoneNumber}", to);
                return false;
            }
        }
    }
}
