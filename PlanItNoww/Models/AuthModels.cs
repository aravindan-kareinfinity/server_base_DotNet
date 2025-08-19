using System.Text.Json.Serialization;
using System.Text.Json;

namespace PlanItNoww.Models
{
    // Login with Email + Password
    public class EmailPasswordLoginReq
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    // Login with Mobile + Password
    public class MobilePasswordLoginReq
    {
        public string mobile { get; set; }
        public string password { get; set; }
    }

    // Login with Mobile + OTP
    public class MobileOTPLoginReq
    {
        public string mobile { get; set; }
        public string otpcode { get; set; }
    }

    // Login with Email + OTP
    public class EmailOTPLoginReq
    {
        public string email { get; set; }
        public string otpcode { get; set; }
    }

    // Google Sign In
    public class GoogleSignInReq
    {
        public string googleid { get; set; }
        public string email { get; set; }
        public string fullname { get; set; }
        public string profileimageurl { get; set; }
    }

    // Facebook Login
    public class FacebookLoginReq
    {
        public string facebookid { get; set; }
        public string email { get; set; }
        public string fullname { get; set; }
        public string profileimageurl { get; set; }
    }

    // Common Authentication Response
    public class AuthResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string accesstoken { get; set; }
        public string refreshtoken { get; set; }
        public Users user { get; set; }
        public UserSession session { get; set; }
        public long expiresin { get; set; }
    }

    // OTP Generation Request
    public class GenerateOTPReq
    {
        public string mobile { get; set; }
        public string email { get; set; }
        public string purpose { get; set; } = "LOGIN";
    }

    // OTP Verification Response
    public class OTPVerificationRes
    {
        public bool success { get; set; }
        public string message { get; set; }
        public bool isvalid { get; set; }
        public string otpcode { get; set; }
    }
}
