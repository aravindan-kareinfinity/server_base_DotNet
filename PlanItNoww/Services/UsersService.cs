using PlanItNoww.Models;
using PlanItNoww.Utils;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Text;

namespace PlanItNoww.Services
{
    public class UsersService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;
        ApplicationEnvironment applicationEnvironment;
        ILogger<UsersService> logger;

        public UsersService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate, IOptions<ApplicationEnvironment> applicationEnvironment, ILogger<UsersService> logger)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
            this.applicationEnvironment = applicationEnvironment.Value;
            this.logger = logger;
        }

        // Authentication Methods

        // Login with Email + Password
        public async Task<AuthResponse> LoginWithEmailPassword(EmailPasswordLoginReq req)
        {
            var response = new AuthResponse();
            
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(req.email) || string.IsNullOrEmpty(req.password))
                {
                    response.success = false;
                    response.message = "Email and password are required";
                    return response;
                }

                if (!SecurityUtils.IsValidEmail(req.email))
                {
                    response.success = false;
                    response.message = "Invalid email format";
                    return response;
                }

                // Find user by email
                var users = await Select(new UsersSelectReq { email = req.email });
                var user = users.FirstOrDefault();

                if (user == null)
                {
                    response.success = false;
                    response.message = "Invalid email or password";
                    return response;
                }

                // Verify password
                if (!SecurityUtils.VerifyPassword(req.password, user.passwordhash, user.salt))
                {
                    response.success = false;
                    response.message = "Invalid email or password";
                    return response;
                }

                // Check if user is active
                if (!user.isactive || user.issuspended)
                {
                    response.success = false;
                    response.message = "Account is inactive or suspended";
                    return response;
                }

                // Generate tokens and session
                var authResult = await CreateUserSession(user);
                response.success = true;
                response.message = "Login successful";
                response.accesstoken = authResult.accesstoken;
                response.refreshtoken = authResult.refreshtoken;
                response.user = authResult.user;
                response.session = authResult.session;
                response.expiresin = 60; // 60 minutes
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "Login failed: " + ex.Message;
            }

            return response;
        }

        // Login with Mobile + Password
        public async Task<AuthResponse> LoginWithMobilePassword(MobilePasswordLoginReq req)
        {
            var response = new AuthResponse();
            
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(req.mobile) || string.IsNullOrEmpty(req.password))
                {
                    response.success = false;
                    response.message = "Mobile and password are required";
                    return response;
                }

                if (!SecurityUtils.IsValidMobile(req.mobile))
                {
                    response.success = false;
                    response.message = "Invalid mobile number format";
                    return response;
                }

                // Find user by mobile
                var users = await Select(new UsersSelectReq { mobile = req.mobile });
                var user = users.FirstOrDefault();

                if (user == null)
                {
                    response.success = false;
                    response.message = "Invalid mobile or password";
                    return response;
                }

                // Verify password
                if (!SecurityUtils.VerifyPassword(req.password, user.passwordhash, user.salt))
                {
                    response.success = false;
                    response.message = "Invalid mobile or password";
                    return response;
                }

                // Check if user is active
                if (!user.isactive || user.issuspended)
                {
                    response.success = false;
                    response.message = "Account is inactive or suspended";
                    return response;
                }

                // Generate tokens and session
                var authResult = await CreateUserSession(user);
                response.success = true;
                response.message = "Login successful";
                response.accesstoken = authResult.accesstoken;
                response.refreshtoken = authResult.refreshtoken;
                response.user = authResult.user;
                response.session = authResult.session;
                response.expiresin = 60; // 60 minutes
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "Login failed: " + ex.Message;
            }

            return response;
        }

        // Login with Email + OTP
        public async Task<AuthResponse> LoginWithEmailOTP(EmailOTPLoginReq req)
        {
            var response = new AuthResponse();
            
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(req.email) || string.IsNullOrEmpty(req.otpcode))
                {
                    response.success = false;
                    response.message = "Email and OTP are required";
                    return response;
                }

                if (!SecurityUtils.IsValidEmail(req.email))
                {
                    response.success = false;
                    response.message = "Invalid email format";
                    return response;
                }

                // Verify OTP
                var otpService = new OTPsService(dbprovider, querybuilderprovider, requeststate);
                var otpReq = new OTPsSelectReq { emailormobile = req.email, otpcode = req.otpcode };
                var otps = await otpService.Select(otpReq);
                var otp = otps.FirstOrDefault();

                if (otp == null || otp.isused || otp.expiresat < DateTime.UtcNow)
                {
                    response.success = false;
                    response.message = "Invalid or expired OTP";
                    return response;
                }

                // Find user by email
                var users = await Select(new UsersSelectReq { email = req.email });
                var user = users.FirstOrDefault();

                if (user == null)
                {
                    response.success = false;
                    response.message = "User not found";
                    return response;
                }

                // Check if user is active
                if (!user.isactive || user.issuspended)
                {
                    response.success = false;
                    response.message = "Account is inactive or suspended";
                    return response;
                }

                // Mark OTP as used
                otp.isused = true;
                await otpService.Update(otp);

                // Generate tokens and session
                var authResult = await CreateUserSession(user);
                response.success = true;
                response.message = "Login successful";
                response.accesstoken = authResult.accesstoken;
                response.refreshtoken = authResult.refreshtoken;
                response.user = authResult.user;
                response.session = authResult.session;
                response.expiresin = 60; // 60 minutes
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "Login failed: " + ex.Message;
            }

            return response;
        }

        // Login with Mobile + OTP
        public async Task<AuthResponse> LoginWithMobileOTP(MobileOTPLoginReq req)
        {
            var response = new AuthResponse();
            
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(req.mobile) || string.IsNullOrEmpty(req.otpcode))
                {
                    response.success = false;
                    response.message = "Mobile and OTP are required";
                    return response;
                }

                if (!SecurityUtils.IsValidMobile(req.mobile))
                {
                    response.success = false;
                    response.message = "Invalid mobile number format";
                    return response;
                }

                // Verify OTP
                var otpService = new OTPsService(dbprovider, querybuilderprovider, requeststate);
                var otpReq = new OTPsSelectReq { emailormobile = req.mobile, otpcode = req.otpcode };
                var otps = await otpService.Select(otpReq);
                var otp = otps.FirstOrDefault();

                if (otp == null || otp.isused || otp.expiresat < DateTime.UtcNow)
                {
                    response.success = false;
                    response.message = "Invalid or expired OTP";
                    return response;
                }

                // Find user by mobile
                var users = await Select(new UsersSelectReq { mobile = req.mobile });
                var user = users.FirstOrDefault();

                if (user == null)
                {
                    response.success = false;
                    response.message = "User not found";
                    return response;
                }

                // Check if user is active
                if (!user.isactive || user.issuspended)
                {
                    response.success = false;
                    response.message = "Account is inactive or suspended";
                    return response;
                }

                // Mark OTP as used
                otp.isused = true;
                await otpService.Update(otp);

                // Generate tokens and session
                var authResult = await CreateUserSession(user);
                response.success = true;
                response.message = "Login successful";
                response.accesstoken = authResult.accesstoken;
                response.refreshtoken = authResult.refreshtoken;
                response.user = authResult.user;
                response.session = authResult.session;
                response.expiresin = 60; // 60 minutes
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "Login failed: " + ex.Message;
            }

            return response;
        }

        // Google Sign In
        public async Task<AuthResponse> GoogleSignIn(GoogleSignInReq req)
        {
            var response = new AuthResponse();
            
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(req.googleid) || string.IsNullOrEmpty(req.email))
                {
                    response.success = false;
                    response.message = "Google ID and email are required";
                    return response;
                }

                if (!SecurityUtils.IsValidEmail(req.email))
                {
                    response.success = false;
                    response.message = "Invalid email format";
                    return response;
                }

                // Check if user exists with this Google ID
                var users = await Select(new UsersSelectReq { googleid = req.googleid });
                var user = users.FirstOrDefault();

                if (user == null)
                {
                    // Check if user exists with this email
                    var emailUsers = await Select(new UsersSelectReq { email = req.email });
                    var emailUser = emailUsers.FirstOrDefault();

                    if (emailUser != null)
                    {
                        // Update existing user with Google ID
                        emailUser.googleid = req.googleid;
                        emailUser.isemailverified = true;
                        await Update(emailUser);
                        user = emailUser;
                    }
                    else
                    {
                        // Create new user
                        user = new Users
                        {
                            email = req.email,
                            googleid = req.googleid,
                            isemailverified = true,
                            isactive = true,
                            roleid = 1, // Default role
                            version = 1
                        };
                        user = await Insert(user);
                    }
                }

                // Check if user is active
                if (!user.isactive || user.issuspended)
                {
                    response.success = false;
                    response.message = "Account is inactive or suspended";
                    return response;
                }

                // Generate tokens and session
                var authResult = await CreateUserSession(user);
                response.success = true;
                response.message = "Google sign-in successful";
                response.accesstoken = authResult.accesstoken;
                response.refreshtoken = authResult.refreshtoken;
                response.user = authResult.user;
                response.session = authResult.session;
                response.expiresin = 60; // 60 minutes
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "Google sign-in failed: " + ex.Message;
            }

            return response;
        }

        // Facebook Login
        public async Task<AuthResponse> FacebookLogin(FacebookLoginReq req)
        {
            var response = new AuthResponse();
            
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(req.facebookid) || string.IsNullOrEmpty(req.email))
                {
                    response.success = false;
                    response.message = "Facebook ID and email are required";
                    return response;
                }

                if (!SecurityUtils.IsValidEmail(req.email))
                {
                    response.success = false;
                    response.message = "Invalid email format";
                    return response;
                }

                // Check if user exists with this Facebook ID
                var users = await Select(new UsersSelectReq { facebookid = req.facebookid });
                var user = users.FirstOrDefault();

                if (user == null)
                {
                    // Check if user exists with this email
                    var emailUsers = await Select(new UsersSelectReq { email = req.email });
                    var emailUser = emailUsers.FirstOrDefault();

                    if (emailUser != null)
                    {
                        // Update existing user with Facebook ID
                        emailUser.facebookid = req.facebookid;
                        emailUser.isemailverified = true;
                        await Update(emailUser);
                        user = emailUser;
                    }
                    else
                    {
                        // Create new user
                        user = new Users
                        {
                            email = req.email,
                            facebookid = req.facebookid,
                            isemailverified = true,
                            isactive = true,
                            roleid = 1, // Default role
                            version = 1
                        };
                        user = await Insert(user);
                    }
                }

                // Check if user is active
                if (!user.isactive || user.issuspended)
                {
                    response.success = false;
                    response.message = "Account is inactive or suspended";
                    return response;
                }

                // Generate tokens and session
                var authResult = await CreateUserSession(user);
                response.success = true;
                response.message = "Facebook login successful";
                response.accesstoken = authResult.accesstoken;
                response.refreshtoken = authResult.refreshtoken;
                response.user = authResult.user;
                response.session = authResult.session;
                response.expiresin = 60; // 60 minutes
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "Facebook login failed: " + ex.Message;
            }

            return response;
        }

        // Generate OTP for mobile or email
        public async Task<OTPVerificationRes> GenerateOTP(GenerateOTPReq req)
        {
            var response = new OTPVerificationRes();
            
            try
            {
                string emailOrMobile = "";
                string deliveryMethod = "";

                // Validate input - either mobile or email should be provided
                if (!string.IsNullOrEmpty(req.mobile))
                {
                    if (!SecurityUtils.IsValidMobile(req.mobile))
                    {
                        response.success = false;
                        response.message = "Invalid mobile number format";
                        return response;
                    }
                    emailOrMobile = req.mobile;
                    deliveryMethod = "SMS";
                }
                else if (!string.IsNullOrEmpty(req.email))
                {
                    if (!SecurityUtils.IsValidEmail(req.email))
                    {
                        response.success = false;
                        response.message = "Invalid email format";
                        return response;
                    }
                    emailOrMobile = req.email;
                    deliveryMethod = "EMAIL";
                }
                else
                {
                    response.success = false;
                    response.message = "Either mobile number or email is required";
                    return response;
                }

                // Generate OTP
                string otpCode = SecurityUtils.GenerateOTP(6);
                
                // Create OTP record
                var otp = new OTPs
                {
                    emailormobile = emailOrMobile,
                    otpcode = otpCode,
                    purpose = req.purpose,
                    expiresat = DateTime.UtcNow.AddMinutes(10), // OTP expires in 10 minutes
                    isused = false,
                    isactive = true,
                    version = 1
                };

                var otpService = new OTPsService(dbprovider, querybuilderprovider, requeststate);
                await otpService.Insert(otp);

                // Send OTP via SMS or Email service based on delivery method
                bool otpSent = false;
                if (deliveryMethod == "EMAIL")
                {
                    // Use EmailService instead of placeholder
                    var emailService = new EmailService(
                        Microsoft.Extensions.Options.Options.Create(applicationEnvironment), 
                        new LoggerFactory().CreateLogger<EmailService>());
                    otpSent = await emailService.SendOTPAsync(emailOrMobile, otpCode, req.purpose);
                }
                else if (deliveryMethod == "SMS")
                {
                    // Use SMSService instead of placeholder
                    var smsService = new SMSService(
                        Microsoft.Extensions.Options.Options.Create(applicationEnvironment), 
                        new LoggerFactory().CreateLogger<SMSService>(), 
                        new HttpClient());
                    otpSent = await smsService.SendOTPAsync(emailOrMobile, otpCode, req.purpose);
                }

                if (!otpSent)
                {
                    response.success = false;
                    response.message = $"Failed to send OTP via {deliveryMethod}";
                    return response;
                }

                response.success = true;
                response.message = $"OTP sent to your {deliveryMethod.ToLower()}";
                response.isvalid = true;
                // response.otpcode = otpCode; // Remove this in production - only for development
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "OTP generation failed: " + ex.Message;
            }

            return response;
        }

        // Create user session and generate tokens
        public async Task<AuthResponse> CreateUserSession(Users user)
        {
            var response = new AuthResponse();
            
            try
            {
                // Generate tokens
                string accessToken = SecurityUtils.GenerateJwtToken(user, applicationEnvironment.jwtsecret, 60);
                string refreshToken = SecurityUtils.GenerateRefreshToken();

                // Update user's refresh token
                user.refreshtoken = refreshToken;
                user.accesstoken = accessToken;
                await Update(user);

                // Create user session
                var session = new UserSession
                {
                    userid = user.id,
                    code = Guid.NewGuid().ToString(),
                    starttime = DateTime.UtcNow,
                    endtime = DateTime.UtcNow.AddHours(24), // Session valid for 24 hours
                    isactive = true,
                    version = 1
                };

                var sessionService = new UserSessionService(dbprovider, querybuilderprovider, requeststate);
                session = await sessionService.Insert(session);

                response.user = user;
                response.session = session;
                response.accesstoken = accessToken;
                response.refreshtoken = refreshToken;
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = "Session creation failed: " + ex.Message;
            }

            return response;
        }

        // Existing methods...
        public async Task<List<Users>> Select(UsersSelectReq req)
        {
            List<Users> result = null;
            using (IDb db = await dbprovider.GetDb())
            {
                await db.Connect();
                result = await this.SelectTransaction(db, req);
            }
            return result;
        }

        public async Task<List<Users>> SelectTransaction(IDb db, UsersSelectReq req)
        {
            List<Users> result = new List<Users>();
            string query = @"
                SELECT id, email, mobile, passwordhash, googleid, facebookid, roleid, pushnotificationtoken, 
                       isemailverified, ismobileverified, isaadhaarverified, version, notes, 
                       createdby, createdon, modifiedby, modifiedon, attributes, isactive, issuspended,
                       salt, accesstoken, refreshtoken
                FROM Users
                ";
            var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
            if (req.id > 0)
            {
                queryBuilder.AddParameter("id", "=", "id", req.id, DbTypes.Types.Long);
            }
            if (!string.IsNullOrEmpty(req.email))
            {
                queryBuilder.AddParameter("email", "=", "email", req.email, DbTypes.Types.String);
            }
            if (!string.IsNullOrEmpty(req.mobile))
            {
                queryBuilder.AddParameter("mobile", "=", "mobile", req.mobile, DbTypes.Types.String);
            }
            if (!string.IsNullOrEmpty(req.googleid))
            {
                queryBuilder.AddParameter("googleid", "=", "googleid", req.googleid, DbTypes.Types.String);
            }
            if (!string.IsNullOrEmpty(req.facebookid))
            {
                queryBuilder.AddParameter("facebookid", "=", "facebookid", req.facebookid, DbTypes.Types.String);
            }
            queryBuilder.AddParameter("isactive", "=", "isactive", true, DbTypes.Types.Boolean);

            queryBuilder.AddOrderBy(QueryBuilder.Order.ASC, "id");
            var command = queryBuilder.GetCommand(db);
            using (DbDataReader reader = await db.Execute(command))
            {
                while (await reader.ReadAsync())
                {
                    Users temp = new Users();
                    temp.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                    temp.email = reader["email"] == DBNull.Value ? "" : reader["email"].ToString();
                    temp.mobile = reader["mobile"] == DBNull.Value ? "" : reader["mobile"].ToString();
                    temp.passwordhash = reader["passwordhash"] == DBNull.Value ? "" : reader["passwordhash"].ToString();
                    temp.googleid = reader["googleid"] == DBNull.Value ? "" : reader["googleid"].ToString();
                    temp.facebookid = reader["facebookid"] == DBNull.Value ? "" : reader["facebookid"].ToString();
                    temp.roleid = reader["roleid"] == DBNull.Value ? 0 : Convert.ToInt64(reader["roleid"]);
                    temp.pushnotificationtoken = reader["pushnotificationtoken"] == DBNull.Value ? "" : reader["pushnotificationtoken"].ToString();
                    temp.isemailverified = reader["isemailverified"] == DBNull.Value ? false : Convert.ToBoolean(reader["isemailverified"]);
                    temp.ismobileverified = reader["ismobileverified"] == DBNull.Value ? false : Convert.ToBoolean(reader["ismobileverified"]);
                    temp.isaadhaarverified = reader["isaadhaarverified"] == DBNull.Value ? false : Convert.ToBoolean(reader["isaadhaarverified"]);
                    temp.version = reader["version"] == DBNull.Value ? 0 : Convert.ToInt32(reader["version"]);
                    temp.notes = reader["notes"] == DBNull.Value ? "" : reader["notes"].ToString();
                    temp.createdby = reader["createdby"] == DBNull.Value ? 0 : Convert.ToInt64(reader["createdby"]);
                    temp.createdon = reader["createdon"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["createdon"]);
                    temp.modifiedby = reader["modifiedby"] == DBNull.Value ? 0 : Convert.ToInt64(reader["modifiedby"]);
                    temp.modifiedon = reader["modifiedon"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["modifiedon"]);
                    temp.attributes_json = reader["attributes"] == DBNull.Value ? "null" : reader["attributes"].ToString();
                    temp.isactive = reader["isactive"] == DBNull.Value ? false : Convert.ToBoolean(reader["isactive"]);
                    temp.issuspended = reader["issuspended"] == DBNull.Value ? false : Convert.ToBoolean(reader["issuspended"]);
                    temp.salt = reader["salt"] == DBNull.Value ? "" : reader["salt"].ToString();
                    temp.accesstoken = reader["accesstoken"] == DBNull.Value ? "" : reader["accesstoken"].ToString();
                    temp.refreshtoken = reader["refreshtoken"] == DBNull.Value ? "" : reader["refreshtoken"].ToString();
                    result.Add(temp);
                }
            }
            return result;
        }

        public async Task<Users> Insert(Users users)
        {
            using (IDb db = await dbprovider.GetDb())
            {
                await db.Connect();
                await this.InsertTransaction(db, users);
            }
            return users;
        }

        public async Task InsertTransaction(IDb db, Users users)
        {
            String query = @"
                INSERT INTO Users (
                    email, mobile, passwordhash, googleid, facebookid, roleid, pushnotificationtoken, 
                    isemailverified, ismobileverified, isaadhaarverified, version, notes, 
                    createdby, createdon, modifiedby, modifiedon, attributes, isactive, issuspended,
                    salt, accesstoken, refreshtoken
                )
                VALUES (
                   @email, @mobile, @passwordhash, @googleid, @facebookid, @roleid, @pushnotificationtoken, 
                   @isemailverified, @ismobileverified, @isaadhaarverified, @version, @notes, 
                   @createdby, @createdon, @modifiedby, @modifiedon, @attributes, @isactive, @issuspended,
                   @salt, @accesstoken, @refreshtoken
                )
                RETURNING id;
                ";
            users.isactive = true;
            users.version = 1;
            users.createdon = DateTime.UtcNow;
            users.createdby = requeststate.usercontext.userid;
            users.modifiedon = DateTime.UtcNow;
            users.modifiedby = requeststate.usercontext.userid;

            // Generate salt and hash password if not provided
            if (string.IsNullOrEmpty(users.salt) && !string.IsNullOrEmpty(users.passwordhash))
            {
                users.salt = SecurityUtils.GenerateSalt();
                users.passwordhash = SecurityUtils.HashPassword(users.passwordhash, users.salt);
            }

            DbCommand command = db.GetCommand(query);

            db.AddParameter(command, "email", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.email) ? "" : users.email;
            db.AddParameter(command, "mobile", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.mobile) ? "" : users.mobile;
            db.AddParameter(command, "passwordhash", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.passwordhash) ? "" : users.passwordhash;
            db.AddParameter(command, "googleid", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.googleid) ? "" : users.googleid;
            db.AddParameter(command, "facebookid", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.facebookid) ? "" : users.facebookid;
            db.AddParameter(command, "roleid", DbTypes.Types.Long).Value = users.roleid;
            db.AddParameter(command, "pushnotificationtoken", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.pushnotificationtoken) ? "" : users.pushnotificationtoken;
            db.AddParameter(command, "isemailverified", DbTypes.Types.Boolean).Value = users.isemailverified;
            db.AddParameter(command, "ismobileverified", DbTypes.Types.Boolean).Value = users.ismobileverified;
            db.AddParameter(command, "isaadhaarverified", DbTypes.Types.Boolean).Value = users.isaadhaarverified;
            db.AddParameter(command, "version", DbTypes.Types.Integer).Value = users.version;
            db.AddParameter(command, "notes", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.notes) ? "" : users.notes;
            db.AddParameter(command, "createdby", DbTypes.Types.Long).Value = users.createdby;
            db.AddParameter(command, "createdon", DbTypes.Types.DateTime).Value = users.createdon;
            db.AddParameter(command, "modifiedby", DbTypes.Types.Long).Value = users.modifiedby;
            db.AddParameter(command, "modifiedon", DbTypes.Types.DateTime).Value = users.modifiedon;
            db.AddParameter(command, "attributes", DbTypes.Types.Json).Value = users.attributes_json;
            db.AddParameter(command, "isactive", DbTypes.Types.Boolean).Value = users.isactive;
            db.AddParameter(command, "issuspended", DbTypes.Types.Boolean).Value = users.issuspended;
            db.AddParameter(command, "salt", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.salt) ? "" : users.salt;
            db.AddParameter(command, "accesstoken", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.accesstoken) ? "" : users.accesstoken;
            db.AddParameter(command, "refreshtoken", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.refreshtoken) ? "" : users.refreshtoken;

            using (DbDataReader reader = await db.Execute(command))
            {
                if (await reader.ReadAsync())
                {
                    users.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                }
            }
        }

        public async Task<Users> Update(Users users)
        {
            using (IDb db = await dbprovider.GetDb())
            {
                await db.Connect();
                await this.UpdateTransaction(db, users);
            }
            return users;
        }

        public async Task<bool> UpdateTransaction(IDb db, Users users)
        {
            bool result = false;
            String query = @"
                UPDATE Users
                SET 
                    email = @email, mobile = @mobile, passwordhash = @passwordhash, googleid = @googleid, 
                    facebookid = @facebookid, roleid = @roleid, pushnotificationtoken = @pushnotificationtoken, 
                    isemailverified = @isemailverified, ismobileverified = @ismobileverified, 
                    isaadhaarverified = @isaadhaarverified, version = @version, notes = @notes, 
                    createdby = @createdby, createdon = @createdon, modifiedby = @modifiedby, 
                    modifiedon = @modifiedon, attributes = @attributes, isactive = @isactive, 
                    issuspended = @issuspended, salt = @salt, accesstoken = @accesstoken, 
                    refreshtoken = @refreshtoken,
                    version = version + 1
                WHERE id = @id
                ";

            var queryBuilder = querybuilderprovider.GetQueryBuilder(query);

            queryBuilder.AddParameter("id", "=", "id", users.id, DbTypes.Types.Long);

            if (users.version > 0)
            {
                queryBuilder.AddParameter("version", "=", "version", users.version, DbTypes.Types.Integer);
            }

            var command = queryBuilder.GetCommand(db);

            users.modifiedon = DateTime.UtcNow;
            users.modifiedby = requeststate.usercontext.userid;

            db.AddParameter(command, "id", DbTypes.Types.Long).Value = users.id;
            db.AddParameter(command, "email", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.email) ? "" : users.email;
            db.AddParameter(command, "mobile", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.mobile) ? "" : users.mobile;
            db.AddParameter(command, "passwordhash", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.passwordhash) ? "" : users.passwordhash;
            db.AddParameter(command, "googleid", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.googleid) ? "" : users.googleid;
            db.AddParameter(command, "facebookid", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.facebookid) ? "" : users.facebookid;
            db.AddParameter(command, "roleid", DbTypes.Types.Long).Value = users.roleid;
            db.AddParameter(command, "pushnotificationtoken", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.pushnotificationtoken) ? "" : users.pushnotificationtoken;
            db.AddParameter(command, "isemailverified", DbTypes.Types.Boolean).Value = users.isemailverified;
            db.AddParameter(command, "ismobileverified", DbTypes.Types.Boolean).Value = users.ismobileverified;
            db.AddParameter(command, "isaadhaarverified", DbTypes.Types.Boolean).Value = users.isaadhaarverified;
            db.AddParameter(command, "version", DbTypes.Types.Integer).Value = users.version;
            db.AddParameter(command, "notes", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.notes) ? "" : users.notes;
            db.AddParameter(command, "createdby", DbTypes.Types.Long).Value = users.createdby;
            db.AddParameter(command, "createdon", DbTypes.Types.DateTime).Value = users.createdon;
            db.AddParameter(command, "modifiedby", DbTypes.Types.Long).Value = users.modifiedby;
            db.AddParameter(command, "modifiedon", DbTypes.Types.DateTime).Value = users.modifiedon;
            db.AddParameter(command, "attributes", DbTypes.Types.Json).Value = users.attributes_json;
            db.AddParameter(command, "isactive", DbTypes.Types.Boolean).Value = users.isactive;
            db.AddParameter(command, "issuspended", DbTypes.Types.Boolean).Value = users.issuspended;
            db.AddParameter(command, "salt", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.salt) ? "" : users.salt;
            db.AddParameter(command, "accesstoken", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.accesstoken) ? "" : users.accesstoken;
            db.AddParameter(command, "refreshtoken", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.refreshtoken) ? "" : users.refreshtoken;

            if (await db.ExecuteNonQuery(command) > 0)
            {
                users.version = users.version + 1;
                result = true;
            }
            return result;
        }

        public async Task<bool> Delete(UsersDeleteReq users)
        {
            bool result = false;
            using (IDb db = await dbprovider.GetDb())
            {
                await db.Connect();
                result = await this.DeleteTransaction(db, users);
            }
            return result;
        }

        public async Task<bool> DeleteTransaction(IDb db, UsersDeleteReq users)
        {
            bool result = false;
            String query = @"
                UPDATE Users
                SET isactive = '0',
                    version = version + 1,
                    modifiedon = @modifiedon,
                    modifiedby = @modifiedby 
                WHERE id = @id
                ";
            var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
            queryBuilder.AddParameter("id", "=", "id", users.id, DbTypes.Types.Long);
            if (users.version > 0)
            {
                queryBuilder.AddParameter("version", "=", "version", users.version, DbTypes.Types.Integer);
            }
            DbCommand command = queryBuilder.GetCommand(db);
            db.AddParameter(command, "id", DbTypes.Types.Long).Value = users.id;
            db.AddParameter(command, "version", DbTypes.Types.Integer).Value = users.version;
            db.AddParameter(command, "modifiedby", DbTypes.Types.Long).Value = requeststate.usercontext.userid;
            db.AddParameter(command, "modifiedon", DbTypes.Types.DateTime).Value = DateTime.UtcNow;
            if (await db.ExecuteNonQuery(command) > 0)
            {
                result = true;
            }
            return result;
        }

        public UsersContext jwtTokenToUserContext(string jwtToken)
        {
            var userContext = new UsersContext();
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(applicationEnvironment.jwtsecret);
                // TODO: Implement JWT token parsing logic
            }
            catch
            {
                // Handle exception
            }
            return userContext;
        }
    }
}
