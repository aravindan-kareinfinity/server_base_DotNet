
namespace PlanItNoww.Utils
{
    public class ApplicationEnvironment
    {
        public string postgresqlconnection { get; set; }
        public ApplicationEnvironmentAwsS3ConfigData awss3config { get; set; } = new ApplicationEnvironmentAwsS3ConfigData();
        public string jwtsecret { get; set; }
        public int jwtExpiryMinutes { get; set; } = 60;
        public int refreshTokenExpiryDays { get; set; } = 30;
        public int sessionExpiryHours { get; set; } = 24;
        
        public FacebookConfig facebook { get; set; } = new FacebookConfig();
        public GoogleConfig google { get; set; } = new GoogleConfig();
        public AadhaarConfig aadhaar { get; set; } = new AadhaarConfig();
        public PaymentConfig payment { get; set; } = new PaymentConfig();
        public SmsConfig sms { get; set; } = new SmsConfig();
        public EmailConfig email { get; set; } = new EmailConfig();
        public PushNotificationConfig pushNotification { get; set; } = new PushNotificationConfig();
        public SecurityConfig security { get; set; } = new SecurityConfig();
        public FileUploadConfig fileUpload { get; set; } = new FileUploadConfig();
        public CachingConfig caching { get; set; } = new CachingConfig();
        public LoggingConfig logging { get; set; } = new LoggingConfig();
        public MonitoringConfig monitoring { get; set; } = new MonitoringConfig();
    }
    
    public class ApplicationEnvironmentAwsS3ConfigData
    {
        public bool iss3enabled { get; set; }
        public string endpoint { get; set; }
        public string accesskey { get; set; }
        public string secretaccesskey { get; set; }
        public string bucketname { get; set; }
        public string path { get; set; }
    }
    
    public class FacebookConfig
    {
        public string appId { get; set; }
        public string appSecret { get; set; }
        public string redirectUri { get; set; }
        public string scope { get; set; }
    }
    
    public class GoogleConfig
    {
        public string clientId { get; set; }
        public string clientSecret { get; set; }
        public string redirectUri { get; set; }
        public string scope { get; set; }
    }
    
    public class AadhaarConfig
    {
        public string apiKey { get; set; }
        public string apiSecret { get; set; }
        public string baseUrl { get; set; }
        public string merchantId { get; set; }
        public bool isProduction { get; set; }
    }
    
    public class PaymentConfig
    {
        public RazorpayConfig razorpay { get; set; } = new RazorpayConfig();
        public StripeConfig stripe { get; set; } = new StripeConfig();
        public PaytmConfig paytm { get; set; } = new PaytmConfig();
        public string defaultGateway { get; set; }
    }
    
    public class RazorpayConfig
    {
        public string keyId { get; set; }
        public string keySecret { get; set; }
        public string webhookSecret { get; set; }
    }
    
    public class StripeConfig
    {
        public string publishableKey { get; set; }
        public string secretKey { get; set; }
        public string webhookSecret { get; set; }
    }
    
    public class PaytmConfig
    {
        public string merchantId { get; set; }
        public string merchantKey { get; set; }
        public string website { get; set; }
        public string industryType { get; set; }
        public string channelId { get; set; }
    }
    
    public class SmsConfig
    {
        public string provider { get; set; }
        public TwilioConfig twilio { get; set; } = new TwilioConfig();
        public Msg91Config msg91 { get; set; } = new Msg91Config();
        public int otpExpiryMinutes { get; set; } = 10;
        public int otpLength { get; set; } = 6;
    }
    
    public class TwilioConfig
    {
        public string accountSid { get; set; }
        public string authToken { get; set; }
        public string fromNumber { get; set; }
    }
    
    public class Msg91Config
    {
        public string authKey { get; set; }
        public string senderId { get; set; }
        public string route { get; set; }
    }
    
    public class EmailConfig
    {
        public string provider { get; set; }
        public SmtpConfig smtp { get; set; } = new SmtpConfig();
        public SendgridConfig sendgrid { get; set; } = new SendgridConfig();
        public EmailTemplates templates { get; set; } = new EmailTemplates();
    }
    
    public class SmtpConfig
    {
        public string host { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool enableSsl { get; set; }
    }
    
    public class SendgridConfig
    {
        public string apiKey { get; set; }
        public string fromEmail { get; set; }
        public string fromName { get; set; }
    }
    
    public class EmailTemplates
    {
        public string welcome { get; set; }
        public string otp { get; set; }
        public string passwordReset { get; set; }
    }
    
    public class PushNotificationConfig
    {
        public FirebaseConfig firebase { get; set; } = new FirebaseConfig();
        public OneSignalConfig onesignal { get; set; } = new OneSignalConfig();
    }
    
    public class FirebaseConfig
    {
        public string serverKey { get; set; }
        public string senderId { get; set; }
    }
    
    public class OneSignalConfig
    {
        public string appId { get; set; }
        public string restApiKey { get; set; }
    }
    
    public class SecurityConfig
    {
        public int passwordMinLength { get; set; } = 8;
        public bool requireSpecialChar { get; set; } = true;
        public bool requireNumber { get; set; } = true;
        public bool requireUppercase { get; set; } = true;
        public int maxLoginAttempts { get; set; } = 5;
        public int lockoutDurationMinutes { get; set; } = 30;
        public int sessionTimeoutMinutes { get; set; } = 60;
    }
    
    public class FileUploadConfig
    {
        public int maxFileSizeMB { get; set; } = 10;
        public string[] allowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
        public string uploadPath { get; set; } = "uploads";
        public string tempPath { get; set; } = "temp";
    }
    
    public class CachingConfig
    {
        public RedisConfig redis { get; set; } = new RedisConfig();
        public int defaultExpiryMinutes { get; set; } = 60;
    }
    
    public class RedisConfig
    {
        public string connectionString { get; set; }
        public int database { get; set; }
    }
    
    public class LoggingConfig
    {
        public bool logToFile { get; set; } = true;
        public bool logToDatabase { get; set; } = false;
        public string logLevel { get; set; } = "Information";
        public int retentionDays { get; set; } = 30;
    }
    
    public class MonitoringConfig
    {
        public bool enableHealthChecks { get; set; } = true;
        public bool enableMetrics { get; set; } = true;
        public bool enableTracing { get; set; } = false;
    }
}
