using PlanItNoww.Models;
using PlanItNoww.Services;
using Microsoft.AspNetCore.Mvc;

namespace PlanItNoww.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        ILogger<UsersController> logger;
        UsersService usersService;
        public UsersController(ILogger<UsersController> logger, UsersService usersService)
        {
            logger = logger;
            this.usersService = usersService;
        }

        // Authentication Endpoints

        [HttpPost("Login/EmailPassword")]
        public async Task<ActionResult<AuthResponse>> LoginWithEmailPassword([FromBody] EmailPasswordLoginReq req)
        {
            try
            {
                var result = await usersService.LoginWithEmailPassword(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Email password login failed");
                return BadRequest(new AuthResponse { success = false, message = "Login failed" });
            }
        }

        [HttpPost("Login/MobilePassword")]
        public async Task<ActionResult<AuthResponse>> LoginWithMobilePassword([FromBody] MobilePasswordLoginReq req)
        {
            try
            {
                var result = await usersService.LoginWithMobilePassword(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mobile password login failed");
                return BadRequest(new AuthResponse { success = false, message = "Login failed" });
            }
        }

        [HttpPost("Login/EmailOTP")]
        public async Task<ActionResult<AuthResponse>> LoginWithEmailOTP([FromBody] EmailOTPLoginReq req)
        {
            try
            {
                var result = await usersService.LoginWithEmailOTP(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Email OTP login failed");
                return BadRequest(new AuthResponse { success = false, message = "Login failed" });
            }
        }

        [HttpPost("Login/MobileOTP")]
        public async Task<ActionResult<AuthResponse>> LoginWithMobileOTP([FromBody] MobileOTPLoginReq req)
        {
            try
            {
                var result = await usersService.LoginWithMobileOTP(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Mobile OTP login failed");
                return BadRequest(new AuthResponse { success = false, message = "Login failed" });
            }
        }

        [HttpPost("Login/Google")]
        public async Task<ActionResult<AuthResponse>> GoogleSignIn([FromBody] GoogleSignInReq req)
        {
            try
            {
                var result = await usersService.GoogleSignIn(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Google sign-in failed");
                return BadRequest(new AuthResponse { success = false, message = "Google sign-in failed" });
            }
        }

        [HttpPost("Login/Facebook")]
        public async Task<ActionResult<AuthResponse>> FacebookLogin([FromBody] FacebookLoginReq req)
        {
            try
            {
                var result = await usersService.FacebookLogin(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Facebook login failed");
                return BadRequest(new AuthResponse { success = false, message = "Facebook login failed" });
            }
        }

        [HttpPost("OTP/Generate")]
        public async Task<ActionResult<OTPVerificationRes>> GenerateOTP([FromBody] GenerateOTPReq req)
        {
            try
            {
                var result = await usersService.GenerateOTP(req);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "OTP generation failed");
                return BadRequest(new OTPVerificationRes { success = false, message = "OTP generation failed" });
            }
        }

        // User Registration
        [HttpPost("Register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] Users user)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrEmpty(user.email) && string.IsNullOrEmpty(user.mobile))
                {
                    return BadRequest(new AuthResponse { success = false, message = "Email or mobile is required" });
                }

                if (string.IsNullOrEmpty(user.passwordhash))
                {
                    return BadRequest(new AuthResponse { success = false, message = "Password is required" });
                }

                // Check if user already exists
                if (!string.IsNullOrEmpty(user.email))
                {
                    var existingUsers = await usersService.Select(new UsersSelectReq { email = user.email });
                    if (existingUsers.Any())
                    {
                        return BadRequest(new AuthResponse { success = false, message = "User with this email already exists" });
                    }
                }

                if (!string.IsNullOrEmpty(user.mobile))
                {
                    var existingUsers = await usersService.Select(new UsersSelectReq { mobile = user.mobile });
                    if (existingUsers.Any())
                    {
                        return BadRequest(new AuthResponse { success = false, message = "User with this mobile already exists" });
                    }
                }

                // Set default values
                user.isactive = true;
                user.version = 1;
                user.roleid = user.roleid > 0 ? user.roleid : 1; // Default role

                // Insert user
                var newUser = await usersService.Insert(user);

                // Create session and generate tokens
                var authResult = await usersService.CreateUserSession(newUser);
                
                var response = new AuthResponse
                {
                    success = true,
                    message = "Registration successful",
                    accesstoken = authResult.accesstoken,
                    refreshtoken = authResult.refreshtoken,
                    user = authResult.user,
                    session = authResult.session,
                    expiresin = 60
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "User registration failed");
                return BadRequest(new AuthResponse { success = false, message = "Registration failed" });
            }
        }

        // Existing CRUD Endpoints

        [HttpGet("Entity")]
        public async Task<ActionResult<ActionRes<Users>>> Entity()
        {
            ActionRes<Users> result = new ActionRes<Users>()
            {
               item = new Users()
            };

            return Ok(result);
        }

        [HttpPost("Select")]
        public async Task<ActionResult<ActionRes<List<Users>>>> Select(ActionReq<UsersSelectReq> req)
        {
            ActionRes<List<Users>> result = new ActionRes<List<Users>>();

            result.item = await usersService.Select(req.item);

            return Ok(result);
        }

        [HttpPost("Insert")]
        public async Task<ActionResult<ActionRes<Users>>> Insert(ActionReq<Users> req)
        {
            ActionRes<Users> result = new ActionRes<Users>();

            result.item = await usersService.Insert(req.item);

            return Ok(result);
        }

        [HttpPost("Update")]
        public async Task<ActionResult<ActionRes<Users>>> Update(ActionReq<Users> req)
        {
            ActionRes<Users> result = new ActionRes<Users>();

            result.item = await usersService.Update(req.item);

            return Ok(result);
        }

        [HttpPost("Save")]
        public async Task<ActionResult<ActionRes<Users>>> Save(ActionReq<Users> req)
        {
            ActionRes<Users> result = new ActionRes<Users>();

            if(req.item.id > 0){
                result.item = await usersService.Update(req.item);
            }else{
                result.item = await usersService.Insert(req.item);
            }

            return Ok(result);
        }

        [HttpPost("Delete")]
        public async Task<ActionResult<ActionRes<bool>>> Delete(ActionReq<UsersDeleteReq> req)
        {
            ActionRes<bool> result = new ActionRes<bool>();

            result.item = await usersService.Delete(req.item);

            return Ok(result);
        }
    }
}
