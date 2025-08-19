using Microsoft.Extensions.Options;
using PlanItNoww.Utils;
using System.Net.Http;
using System.Text.Json;

namespace PlanItNoww.Services
{
    public interface IPushNotificationService
    {
        Task<bool> SendNotificationAsync(PushNotificationRequest request);
        Task<bool> SendNotificationToUserAsync(long userId, string title, string message, Dictionary<string, object> data = null);
        Task<bool> SendNotificationToMultipleUsersAsync(List<long> userIds, string title, string message, Dictionary<string, object> data = null);
        Task<bool> SendNotificationToTopicAsync(string topic, string title, string message, Dictionary<string, object> data = null);
    }

    public class PushNotificationRequest
    {
        public string title { get; set; }
        public string message { get; set; }
        public Dictionary<string, object> data { get; set; } = new();
        public string? imageUrl { get; set; }
        public string? clickAction { get; set; }
        public string? sound { get; set; } = "default";
        public int? badge { get; set; }
        public string? priority { get; set; } = "high";
        public int? timeToLive { get; set; } = 2419200; // 28 days in seconds
    }

    public class PushNotificationService : IPushNotificationService
    {
        private readonly ApplicationEnvironment _config;
        private readonly ILogger<PushNotificationService> _logger;
        private readonly HttpClient _httpClient;

        public PushNotificationService(IOptions<ApplicationEnvironment> config, ILogger<PushNotificationService> logger, HttpClient httpClient)
        {
            _config = config.Value;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<bool> SendNotificationAsync(PushNotificationRequest request)
        {
            try
            {
                // Try Firebase first, then OneSignal as fallback
                var firebaseResult = await SendViaFirebaseAsync(request);
                if (firebaseResult)
                {
                    return true;
                }

                var oneSignalResult = await SendViaOneSignalAsync(request);
                return oneSignalResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send push notification");
                return false;
            }
        }

        public async Task<bool> SendNotificationToUserAsync(long userId, string title, string message, Dictionary<string, object> data = null)
        {
            try
            {
                var request = new PushNotificationRequest
                {
                    title = title,
                    message = message,
                    data = data ?? new Dictionary<string, object>()
                };

                // TODO: Get user's FCM token from database
                // For now, this is a placeholder implementation
                return await SendNotificationAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> SendNotificationToMultipleUsersAsync(List<long> userIds, string title, string message, Dictionary<string, object> data = null)
        {
            try
            {
                var request = new PushNotificationRequest
                {
                    title = title,
                    message = message,
                    data = data ?? new Dictionary<string, object>()
                };

                // TODO: Get FCM tokens for multiple users from database
                // For now, this is a placeholder implementation
                return await SendNotificationAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to multiple users");
                return false;
            }
        }

        public async Task<bool> SendNotificationToTopicAsync(string topic, string title, string message, Dictionary<string, object> data = null)
        {
            try
            {
                var request = new PushNotificationRequest
                {
                    title = title,
                    message = message,
                    data = data ?? new Dictionary<string, object>()
                };

                // Firebase supports topic-based messaging
                return await SendViaFirebaseAsync(request, topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to topic {Topic}", topic);
                return false;
            }
        }

        #region Firebase Implementation
        private async Task<bool> SendViaFirebaseAsync(PushNotificationRequest request, string topic = null)
        {
            try
            {
                var firebaseConfig = _config.pushNotification.firebase;
                
                var fcmMessage = new
                {
                    to = topic != null ? $"/topics/{topic}" : null, // Topic-based messaging
                    notification = new
                    {
                        title = request.title,
                        body = request.message,
                        icon = "ic_notification",
                        click_action = request.clickAction ?? "FLUTTER_NOTIFICATION_CLICK",
                        sound = request.sound,
                        badge = request.badge
                    },
                    data = request.data,
                    priority = request.priority,
                    time_to_live = request.timeToLive
                };

                var jsonContent = JsonSerializer.Serialize(fcmMessage);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("key", firebaseConfig.serverKey);

                var response = await _httpClient.PostAsync("https://fcm.googleapis.com/fcm/send", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var fcmResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    if (fcmResponse.TryGetProperty("success", out var success) && success.GetInt32() > 0)
                    {
                        _logger.LogInformation("Firebase notification sent successfully");
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("Firebase notification failed: {Response}", responseContent);
                        return false;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Firebase notification failed with status: {StatusCode}, Error: {Error}", 
                        response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send Firebase notification");
                return false;
            }
        }
        #endregion

        #region OneSignal Implementation
        private async Task<bool> SendViaOneSignalAsync(PushNotificationRequest request)
        {
            try
            {
                var oneSignalConfig = _config.pushNotification.onesignal;
                
                var oneSignalMessage = new
                {
                    app_id = oneSignalConfig.appId,
                    included_segments = new[] { "All" }, // Send to all users
                    headings = new { en = request.title },
                    contents = new { en = request.message },
                    data = request.data,
                    large_icon = request.imageUrl,
                    url = request.clickAction,
                    sound = request.sound,
                    priority = request.priority == "high" ? 10 : 5,
                    ttl = request.timeToLive
                };

                var jsonContent = JsonSerializer.Serialize(oneSignalMessage);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {oneSignalConfig.restApiKey}");

                var response = await _httpClient.PostAsync("https://onesignal.com/api/v1/notifications", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var oneSignalResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    if (oneSignalResponse.TryGetProperty("id", out var notificationId))
                    {
                        _logger.LogInformation("OneSignal notification sent successfully with ID: {NotificationId}", 
                            notificationId.GetString());
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("OneSignal notification failed: {Response}", responseContent);
                        return false;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("OneSignal notification failed with status: {StatusCode}, Error: {Error}", 
                        response.StatusCode, errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OneSignal notification");
                return false;
            }
        }
        #endregion
    }
}
