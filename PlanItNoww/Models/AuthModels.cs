using System.ComponentModel.DataAnnotations;

namespace PlanItNoww.Models
{
    // Existing Login Models
    public class EmailPasswordLoginReq
    {
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }

    public class MobilePasswordLoginReq
    {
        [Required]
        public string mobile { get; set; }
        [Required]
        public string password { get; set; }
    }

    public class MobileOTPLoginReq
    {
        [Required]
        public string mobile { get; set; }
        [Required]
        public string otpcode { get; set; }
    }

    public class EmailOTPLoginReq
    {
        [Required]
        public string email { get; set; }
        [Required]
        public string otpcode { get; set; }
    }

    public class GoogleSignInReq
    {
        [Required]
        public string idToken { get; set; }
        public string accessToken { get; set; }
    }

    public class FacebookLoginReq
    {
        [Required]
        public string accessToken { get; set; }
        public string userId { get; set; }
    }

    // New Signup Models
    public class MobileSignupReq
    {
        [Required]
        public string mobile { get; set; }
        [Required]
        public string otpcode { get; set; }
        [Required]
        public string fullName { get; set; }
        public string email { get; set; }
        public DateTime? dateOfBirth { get; set; }
        public string gender { get; set; }
        public string profilePicture { get; set; }
    }

    public class GmailSignupReq
    {
        [Required]
        public string idToken { get; set; }
        public string accessToken { get; set; }
        [Required]
        public string fullName { get; set; }
        public string mobile { get; set; }
        public DateTime? dateOfBirth { get; set; }
        public string gender { get; set; }
        public string profilePicture { get; set; }
    }

    // GetOTP Request Model
    public class GetOTPReq
    {
        [Required]
        public string mobile { get; set; }
        public string email { get; set; }
        [Required]
        public string purpose { get; set; } = "SIGNUP"; // SIGNUP, LOGIN, PASSWORD_RESET, VERIFICATION
    }

    // OTP Generation Request (existing)
    public class GenerateOTPReq
    {
        public string mobile { get; set; }
        public string email { get; set; }
        public string purpose { get; set; } = "LOGIN";
    }

    // Response Models
    public class AuthResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string accesstoken { get; set; }
        public string refreshtoken { get; set; }
        public Users user { get; set; }
        public UserSession session { get; set; }
        public int expiresin { get; set; }
    }

    public class OTPVerificationRes
    {
        public bool success { get; set; }
        public string message { get; set; }
        public bool isvalid { get; set; }
        public string otpcode { get; set; }
    }

    public class SignupResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string accesstoken { get; set; }
        public string refreshtoken { get; set; }
        public Users user { get; set; }
        public UserSession session { get; set; }
        public int expiresin { get; set; }
    }
}
