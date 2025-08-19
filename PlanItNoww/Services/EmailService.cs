using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using PlanItNoww.Utils;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PlanItNoww.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task<bool> SendOTPAsync(string to, string otpCode, string purpose = "LOGIN");
        Task<bool> SendWelcomeEmailAsync(string to, string userName);
        Task<bool> SendPasswordResetEmailAsync(string to, string resetLink);
    }

    public class EmailService : IEmailService
    {
        private readonly ApplicationEnvironment _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<ApplicationEnvironment> config, ILogger<EmailService> logger)
        {
            _config = config.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                if (_config.email.provider.ToLower() == "sendgrid")
                {
                    return await SendViaSendGridAsync(to, subject, body, isHtml);
                }
                else
                {
                    return await SendViaSMTPAsync(to, subject, body, isHtml);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", to);
                return false;
            }
        }

        private async Task<bool> SendViaSMTPAsync(string to, string subject, string body, bool isHtml)
        {
            try
            {
                var smtpConfig = _config.email.smtp;
                
                using var client = new SmtpClient(smtpConfig.host, smtpConfig.port)
                {
                    EnableSsl = smtpConfig.enableSsl,
                    Credentials = new NetworkCredential(smtpConfig.username, smtpConfig.password)
                };

                var message = new MailMessage
                {
                    From = new MailAddress(smtpConfig.username),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };
                message.To.Add(to);

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully via SMTP to {Email}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email via SMTP to {Email}", to);
                return false;
            }
        }

        private async Task<bool> SendViaSendGridAsync(string to, string subject, string body, bool isHtml)
        {
            try
            {
                var sendGridConfig = _config.email.sendgrid;
                var client = new SendGridClient(sendGridConfig.apiKey);
                
                var from = new EmailAddress(sendGridConfig.fromEmail, sendGridConfig.fromName);
                var toEmail = new EmailAddress(to);
                
                var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, 
                    isHtml ? null : body, isHtml ? body : null);
                
                var response = await client.SendEmailAsync(msg);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully via SendGrid to {Email}", to);
                    return true;
                }
                else
                {
                    _logger.LogError("SendGrid failed with status: {StatusCode}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email via SendGrid to {Email}", to);
                return false;
            }
        }

        public async Task<bool> SendOTPAsync(string to, string otpCode, string purpose = "LOGIN")
        {
            var subject = $"Your OTP for {purpose}";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                        <h2 style='color: #007bff; text-align: center;'>Your OTP Code</h2>
                        <div style='background-color: white; padding: 30px; border-radius: 8px; text-align: center;'>
                            <h1 style='color: #28a745; font-size: 48px; margin: 20px 0;'>{otpCode}</h1>
                            <p style='color: #6c757d; font-size: 16px;'>This OTP will expire in 10 minutes.</p>
                            <p style='color: #6c757d; font-size: 14px;'>If you didn't request this OTP, please ignore this email.</p>
                        </div>
                        <div style='text-align: center; margin-top: 20px;'>
                            <p style='color: #6c757d; font-size: 12px;'>© 2024 PlanItNoww. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body, true);
        }

        public async Task<bool> SendWelcomeEmailAsync(string to, string userName)
        {
            var subject = "Welcome to PlanItNoww!";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                        <h2 style='color: #007bff; text-align: center;'>Welcome to PlanItNoww!</h2>
                        <div style='background-color: white; padding: 30px; border-radius: 8px;'>
                            <p style='color: #333; font-size: 16px;'>Hello {userName},</p>
                            <p style='color: #333; font-size: 16px;'>Welcome to PlanItNoww! We're excited to have you on board.</p>
                            <p style='color: #333; font-size: 16px;'>Your account has been successfully created and you can now start using our services.</p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='https://yourdomain.com/login' style='background-color: #007bff; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>Get Started</a>
                            </div>
                        </div>
                        <div style='text-align: center; margin-top: 20px;'>
                            <p style='color: #6c757d; font-size: 12px;'>© 2024 PlanItNoww. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body, true);
        }

        public async Task<bool> SendPasswordResetEmailAsync(string to, string resetLink)
        {
            var subject = "Password Reset Request";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 10px;'>
                        <h2 style='color: #dc3545; text-align: center;'>Password Reset Request</h2>
                        <div style='background-color: white; padding: 30px; border-radius: 8px;'>
                            <p style='color: #333; font-size: 16px;'>You have requested to reset your password.</p>
                            <p style='color: #333; font-size: 16px;'>Click the button below to reset your password:</p>
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{resetLink}' style='background-color: #dc3545; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>Reset Password</a>
                            </div>
                            <p style='color: #6c757d; font-size: 14px;'>This link will expire in 1 hour. If you didn't request this, please ignore this email.</p>
                        </div>
                        <div style='text-align: center; margin-top: 20px;'>
                            <p style='color: #6c757d; font-size: 12px;'>© 2024 PlanItNoww. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            return await SendEmailAsync(to, subject, body, true);
        }
    }
}
