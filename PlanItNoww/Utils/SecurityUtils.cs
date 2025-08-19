using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using PlanItNoww.Models;

namespace PlanItNoww.Utils
{
    public static class SecurityUtils
    {
        private const int SALT_SIZE = 16;
        private const int HASH_SIZE = 32;
        private const int ITERATIONS = 10000;

        // Generate a random salt
        public static string GenerateSalt()
        {
            byte[] salt = new byte[SALT_SIZE];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        // Hash password with salt using PBKDF2
        public static string HashPassword(string password, string salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), ITERATIONS, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HASH_SIZE);
                return Convert.ToBase64String(hash);
            }
        }

        // Verify password against stored hash and salt
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            try
            {
                string computedHash = HashPassword(password, storedSalt);
                return computedHash == storedHash;
            }
            catch
            {
                return false;
            }
        }

        // Generate JWT token
        public static string GenerateJwtToken(Users user, string secretKey, long expiresInMinutes = 60)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Email, user.email ?? ""),
                new Claim(ClaimTypes.MobilePhone, user.mobile ?? ""),
                new Claim("userid", user.id.ToString()),
                new Claim("roleid", user.roleid.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Generate refresh token
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        // Generate OTP code
        public static string GenerateOTP(int length = 6)
        {
            Random random = new Random();
            string otp = "";
            for (int i = 0; i < length; i++)
            {
                otp += random.Next(0, 10).ToString();
            }
            return otp;
        }

        // Validate email format
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Validate mobile number format (basic validation)
        public static bool IsValidMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return false;
            
            // Remove any non-digit characters
            string digitsOnly = new string(mobile.Where(char.IsDigit).ToArray());
            
            // Check if it's between 10-15 digits (international standard)
            return digitsOnly.Length >= 10 && digitsOnly.Length <= 15;
        }

        // Send OTP via Email (placeholder method)
        public static async Task<bool> SendEmailOTP(string email, string otpCode, string purpose = "LOGIN")
        {
            // TODO: Implement actual email sending logic using SMTP or SendGrid
            // For now, this is a placeholder that always returns true
            
            try
            {
                // Example email content
                string subject = $"Your OTP for {purpose}";
                string body = $@"
                    <html>
                    <body>
                        <h2>Your OTP Code</h2>
                        <p>Your OTP for {purpose.ToLower()} is: <strong>{otpCode}</strong></p>
                        <p>This OTP will expire in 10 minutes.</p>
                        <p>If you didn't request this OTP, please ignore this email.</p>
                    </body>
                    </html>
                ";

                // TODO: Replace with actual email service implementation
                // await SendEmailAsync(email, subject, body);
                
                Console.WriteLine($"Email OTP sent to {email}: {otpCode}"); // For development only
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email OTP: {ex.Message}");
                return false;
            }
        }

        // Send OTP via SMS (placeholder method)
        public static async Task<bool> SendSMSOTP(string mobile, string otpCode, string purpose = "LOGIN")
        {
            // TODO: Implement actual SMS sending logic using Twilio or MSG91
            // For now, this is a placeholder that always returns true
            
            try
            {
                string message = $"Your OTP for {purpose} is: {otpCode}. Valid for 10 minutes. Do not share with anyone.";
                
                // TODO: Replace with actual SMS service implementation
                // await SendSMSAsync(mobile, message);
                
                Console.WriteLine($"SMS OTP sent to {mobile}: {otpCode}"); // For development only
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send SMS OTP: {ex.Message}");
                return false;
            }
        }
    }
}
