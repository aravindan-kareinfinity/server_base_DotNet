using PlanItNoww.Models;
using PlanItNoww.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlanItNoww.Services
{
    public class UsersService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;
        UserSessionService usersessionservice;
        ApplicationEnvironment applicationenvironment;
        ILogger<UsersService> logger;

        public UsersService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate, 
            UserSessionService usersessionservice, IOptions<ApplicationEnvironment> applicationenvironment, 
            ILogger<UsersService> logger)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
            this.usersessionservice = usersessionservice;
            this.applicationenvironment = applicationenvironment.Value;
            this.logger = logger;
        }

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
                SELECT Users.id,Users.name,Users.email,Users.mobile,Users.mobilecountrycode,Users.designation,Users.otp,Users.otpexpirationtime,Users.version,Users.createdby,Users.createdon,Users.modifiedby,Users.modifiedon,Users.attributes,Users.isactive,Users.issuspended,Users.parentid,Users.isfactory,Users.notes
                FROM Users
                ";
            var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
            if (req.id > 0)
            {
                queryBuilder.AddParameter("Users.id", "=", "id", req.id, DbTypes.Types.Long);
            }
            if (!String.IsNullOrEmpty(req.mobile))
            {
                queryBuilder.AddParameter("Users.mobile", "=", "mobile", req.mobile, DbTypes.Types.String);
            }
            queryBuilder.AddParameter("Users.isactive", "=", "isactive", true, DbTypes.Types.Boolean);

            queryBuilder.AddOrderBy(QueryBuilder.Order.ASC, "Users.id");
            var command = queryBuilder.GetCommand(db);
            using (DbDataReader reader = await db.Execute(command))
            {
                while (await reader.ReadAsync())
                {
                    Users temp = new Users();
                    temp.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                    temp.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
                    temp.email = reader["email"] == DBNull.Value ? "" : reader["email"].ToString();
                    temp.mobile = reader["mobile"] == DBNull.Value ? "" : reader["mobile"].ToString();
                    temp.mobilecountrycode = reader["mobilecountrycode"] == DBNull.Value ? "" : reader["mobilecountrycode"].ToString();
                    temp.designation = reader["designation"] == DBNull.Value ? "" : reader["designation"].ToString();
                    temp.otp = reader["otp"] == DBNull.Value ? "" : reader["otp"].ToString();
                    temp.otpexpirationtime = reader["otpexpirationtime"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["otpexpirationtime"]);
                    temp.version = reader["version"] == DBNull.Value ? 0 : Convert.ToInt32(reader["version"]);
                    temp.createdby = reader["createdby"] == DBNull.Value ? 0 : Convert.ToInt64(reader["createdby"]);
                    temp.createdon = reader["createdon"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["createdon"]);
                    temp.modifiedby = reader["modifiedby"] == DBNull.Value ? 0 : Convert.ToInt64(reader["modifiedby"]);
                    temp.modifiedon = reader["modifiedon"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["modifiedon"]);
                    temp.attributes_json = reader["attributes"] == DBNull.Value ? "null" : reader["attributes"].ToString();
                    temp.isactive = reader["isactive"] == DBNull.Value ? false : Convert.ToBoolean(reader["isactive"]);
                    temp.issuspended = reader["issuspended"] == DBNull.Value ? false : Convert.ToBoolean(reader["issuspended"]);
                    temp.parentid = reader["parentid"] == DBNull.Value ? 0 : Convert.ToInt64(reader["parentid"]);
                    temp.isfactory = reader["isfactory"] == DBNull.Value ? false : Convert.ToBoolean(reader["isfactory"]);
                    temp.notes = reader["notes"] == DBNull.Value ? "" : reader["notes"].ToString();
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
                    name,email,mobile,mobilecountrycode,designation,otp,otpexpirationtime,version,createdby,createdon,modifiedby,modifiedon,attributes,isactive,issuspended,parentid,isfactory,notes
                )
                VALUES (
                   @name,@email,@mobile,@mobilecountrycode,@designation,@otp,@otpexpirationtime,@version,@createdby,@createdon,@modifiedby,@modifiedon,@attributes,@isactive,@issuspended,@parentid,@isfactory,@notes
                )
                RETURNING id;
                ";
            users.isactive = true;
            users.version = 1;
            users.createdon = DateTime.UtcNow;
            users.createdby = requeststate.usercontext.userid;
            users.modifiedon = DateTime.UtcNow;
            users.modifiedby = requeststate.usercontext.userid;

            DbCommand command = db.GetCommand(query);

            db.AddParameter(command, "name", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.name) ? "" : users.name;
            db.AddParameter(command, "email", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.email) ? "" : users.email;
            db.AddParameter(command, "mobile", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.mobile) ? "" : users.mobile;
            db.AddParameter(command, "mobilecountrycode", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.mobilecountrycode) ? "" : users.mobilecountrycode;
            db.AddParameter(command, "designation", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.designation) ? "" : users.designation;
            db.AddParameter(command, "otp", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.otp) ? "" : users.otp;
            db.AddParameter(command, "otpexpirationtime", DbTypes.Types.DateTime).Value = users.otpexpirationtime;
            db.AddParameter(command, "version", DbTypes.Types.Integer).Value = users.version;
            db.AddParameter(command, "createdby", DbTypes.Types.Long).Value = users.createdby;
            db.AddParameter(command, "createdon", DbTypes.Types.DateTime).Value = users.createdon;
            db.AddParameter(command, "modifiedby", DbTypes.Types.Long).Value = users.modifiedby;
            db.AddParameter(command, "modifiedon", DbTypes.Types.DateTime).Value = users.modifiedon;
            db.AddParameter(command, "attributes", DbTypes.Types.Json).Value = users.attributes_json;
            db.AddParameter(command, "isactive", DbTypes.Types.Boolean).Value = users.isactive;
            db.AddParameter(command, "issuspended", DbTypes.Types.Boolean).Value = users.issuspended;
            db.AddParameter(command, "parentid", DbTypes.Types.Long).Value = users.parentid;
            db.AddParameter(command, "isfactory", DbTypes.Types.Boolean).Value = users.isfactory;
            db.AddParameter(command, "notes", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.notes) ? "" : users.notes;

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
                        name = @name,email = @email,mobile = @mobile,mobilecountrycode = @mobilecountrycode,designation = @designation,otp = @otp,otpexpirationtime = @otpexpirationtime,modifiedby = @modifiedby,modifiedon = @modifiedon,attributes = @attributes,issuspended = @issuspended,parentid = @parentid,isfactory = @isfactory,notes = @notes,
                        version = version + 1
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
            db.AddParameter(command, "name", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.name) ? "" : users.name;
            db.AddParameter(command, "email", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.email) ? "" : users.email;
            db.AddParameter(command, "mobile", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.mobile) ? "" : users.mobile;
            db.AddParameter(command, "mobilecountrycode", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.mobilecountrycode) ? "" : users.mobilecountrycode;
            db.AddParameter(command, "designation", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.designation) ? "" : users.designation;
            db.AddParameter(command, "otp", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.otp) ? "" : users.otp;
            db.AddParameter(command, "otpexpirationtime", DbTypes.Types.DateTime).Value = users.otpexpirationtime;
            db.AddParameter(command, "version", DbTypes.Types.Integer).Value = users.version;
            db.AddParameter(command, "modifiedby", DbTypes.Types.Long).Value = users.modifiedby;
            db.AddParameter(command, "modifiedon", DbTypes.Types.DateTime).Value = users.modifiedon;
            db.AddParameter(command, "attributes", DbTypes.Types.Json).Value = users.attributes_json;
            db.AddParameter(command, "issuspended", DbTypes.Types.Boolean).Value = users.issuspended;
            db.AddParameter(command, "parentid", DbTypes.Types.Long).Value = users.parentid;
            db.AddParameter(command, "isfactory", DbTypes.Types.Boolean).Value = users.isfactory;
            db.AddParameter(command, "notes", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.notes) ? "" : users.notes;

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

        public UsersContext JwtTokenToUserContext(String token)
        {
            var usercontext = new UsersContext();
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(applicationenvironment.jwtsecret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                usercontext = new UsersContext()
                {
                    userid = long.Parse(jwtToken.Claims.First(x => x.Type == "userid").Value),
                    usermobile = jwtToken.Claims.First(x => x.Type == "usermobile").Value,
                    username = jwtToken.Claims.First(x => x.Type == "username").Value,
                    useremail = jwtToken.Claims.First(x => x.Type == "useremail").Value,
                    userpermission = HexToPermission(jwtToken.Claims.First(x => x.Type == "permissionhex").Value),
                };
            }
            catch (SecurityTokenExpiredException e)
            {
                logger.LogInformation($"Token expired: {e.Message}");
            }
            catch (Exception e)
            {
                logger.LogInformation($"Token validation error: {e.Message}");
            }
            return usercontext;
        }

        public string GenerateJwtToken(UserGenerateJwtTokenReq req)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(applicationenvironment.jwtsecret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                 new Claim("userid", req.userid.ToString()),
                 new Claim("usermobile", req.usermobile.ToString()),
                 new Claim("username", req.username.ToString()),
                 new Claim("useremail", req.useremail.ToString()),
                 new Claim("permissionhex", req.permissionhex.ToString()),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddYears(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string PermissionToHex(UsersPermissionData req)
        {
            StringBuilder binary = new StringBuilder();
            binary.Append("1");
            binary.Append(req.createstaff.view ? "1" : "0");
            binary.Append(req.createstaff.manage ? "1" : "0");
            binary.Append(req.creategroup.view ? "1" : "0");
            binary.Append(req.creategroup.manage ? "1" : "0");
            binary.Append(req.approveusersingroup.view ? "1" : "0");
            binary.Append(req.approveusersingroup.manage ? "1" : "0");
            binary.Append(req.createmessage.view ? "1" : "0");
            binary.Append(req.createmessage.manage ? "1" : "0");
            binary.Append(req.createtask.view ? "1" : "0");
            binary.Append(req.createtask.manage ? "1" : "0");
            var hex = BinaryToHex(binary.ToString());
            return hex;
        }

        public UsersPermissionData HexToPermission(string req)
        {
            var result = new UsersPermissionData();

            string binary = HexToBinary(req);
            Queue<char> queue = new Queue<char>(binary);
            queue.Dequeue();

            result.createstaff.view = queue.Dequeue().ToString() == "1";
            result.createstaff.manage = queue.Dequeue().ToString() == "1";
            result.creategroup.view = queue.Dequeue().ToString() == "1";
            result.creategroup.manage = queue.Dequeue().ToString() == "1";
            result.approveusersingroup.view = queue.Dequeue().ToString() == "1";
            result.approveusersingroup.manage = queue.Dequeue().ToString() == "1";
            result.createmessage.view = queue.Dequeue().ToString() == "1";
            result.createmessage.manage = queue.Dequeue().ToString() == "1";
            result.createtask.view = queue.Dequeue().ToString() == "1";
            result.createtask.manage = queue.Dequeue().ToString() == "1";

            return result;
        }

        string BinaryToHex(string binaryString)
        {
            if (string.IsNullOrEmpty(binaryString))
            {
                throw new ArgumentException("Binary string must not be null or empty.");
            }

            int remainder = binaryString.Length % 4;
            if (remainder != 0)
            {
                binaryString = binaryString.PadLeft(binaryString.Length + (4 - remainder), '0');
            }

            int numBits = binaryString.Length;
            string hexString = "";

            for (int i = 0; i < numBits; i += 4)
            {
                string nibble = binaryString.Substring(i, 4);
                int decimalValue = Convert.ToInt32(nibble, 2);
                hexString += decimalValue.ToString("X");
            }

            return hexString;
        }

        string HexToBinary(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
            {
                throw new ArgumentException("Hexadecimal string must not be null or empty.");
            }

            string binaryString = "";

            foreach (char hexChar in hexString)
            {
                int decimalValue = Convert.ToInt32(hexChar.ToString(), 16);
                string binaryNibble = Convert.ToString(decimalValue, 2).PadLeft(4, '0');
                binaryString += binaryNibble;
            }

            return binaryString.TrimStart('0');
        }

        public async Task<UsersGetOtpRes> GetOtp(UsersGetOtpReq req)
        {
            using (IDb db = await dbprovider.GetDb())
            {
                await db.Connect();
                return await GetOtpTransaction(db, req);
            }
        }

        public async Task<UsersGetOtpRes> GetOtpTransaction(IDb db, UsersGetOtpReq req)
        {
            var result = new UsersGetOtpRes();
            var user = (await SelectTransaction(db, new UsersSelectReq
            {
                mobile = req.mobile,
            })).FirstOrDefault();

            if (user == null)
            {
                throw new AppException(AppException.ErrorCodes.UserNotFound);
            }

            user.otp = GenerateRandomNumber();
            user.otpexpirationtime = DateTime.UtcNow.AddMinutes(5);
            await UpdateTransaction(db, user);
            result.mobile = user.mobile;
            result.name = user.name;

            return result;
        }

        public string GenerateRandomNumber()
        {
            Random random = new Random();
            int randomNumber = random.Next(1000, 10000);
            return randomNumber.ToString();
        }

        public async Task<UsersContext> Login(UsersLoginReq req)
        {
            UsersContext result;
            using (IDb db = await dbprovider.GetDb())
            {
                await db.Connect();
                try
                {
                    await db.BeginTransaction();
                    result = await LoginTransaction(db, req);
                    await db.CommitTransaction();
                }
                catch (Exception)
                {
                    await db.RollbackTransaction();
                    throw;
                }
            }
            return result;
        }

        public async Task<UsersContext> LoginTransaction(IDb db, UsersLoginReq req)
        {
            var result = new UsersContext();
            var user = (await SelectTransaction(db, new UsersSelectReq
            {
                mobile = req.mobile
            })).FirstOrDefault();

            if (user == null)
            {
                throw new AppException(AppException.ErrorCodes.UserNotFound);
            }

            if (user.otpexpirationtime.CompareTo(DateTime.UtcNow) < 0)
            {
                throw new AppException(AppException.ErrorCodes.OtpExpired);
            }

            if (user.otp != req.otp)
            {
                throw new AppException(AppException.ErrorCodes.OtpInvalid);
            }

            var usersession = new UserSession
            {
                userid = user.id,
                code = Guid.NewGuid().ToString(),
                starttime = DateTime.UtcNow,
                endtime = DateTime.UtcNow.AddYears(1),
            };
            await usersessionservice.InsertTransaction(db, usersession);

            result.userid = user.id;
            result.usermobile = user.mobile;
            result.username = user.name;
            result.useremail = user.email;
            result.userpermission = user.attributes.permission;
            result.refreshtoken = usersession.code;
            result.accesstoken = GenerateJwtToken(new UserGenerateJwtTokenReq
            {
                userid = user.id,
                usermobile = user.mobile,
                useremail = user.email,
                username = user.name,
                permissionhex = PermissionToHex(user.attributes.permission)
            });

            return result;
        }

        public async Task<UsersContext> RefreshToken(UsersRefereshTokenReq req)
        {
            UsersContext result;
            using (IDb db = await dbprovider.GetDb())
            {
                await db.Connect();
                try
                {
                    await db.BeginTransaction();
                    result = await RefreshTokenTransaction(db, req);
                    await db.CommitTransaction();
                }
                catch (Exception)
                {
                    await db.RollbackTransaction();
                    throw;
                }
            }
            return result;
        }

        public async Task<UsersContext> RefreshTokenTransaction(IDb db, UsersRefereshTokenReq req)
        {
            var result = new UsersContext();

            var usersession = (await usersessionservice.SelectTransaction(db, new UserSessionSelectReq
            {
                userid = req.userid,
                code = req.refreshtoken
            })).FirstOrDefault();

            if (usersession == null)
            {
                throw new AppException(AppException.ErrorCodes.SessionInvalid);
            }

            if (usersession.endtime.CompareTo(DateTime.UtcNow) < 0)
            {
                throw new AppException(AppException.ErrorCodes.SessionExpired);
            }

            usersession.code = Guid.NewGuid().ToString();
            usersession.endtime = DateTime.UtcNow.AddYears(1);
            await usersessionservice.UpdateTransaction(db, usersession);

            var user = (await SelectTransaction(db, new UsersSelectReq
            {
                id = req.userid,
            })).First();

            result.userid = user.id;
            result.usermobile = user.mobile;
            result.username = user.name;
            result.useremail = user.email;
            result.userpermission = user.attributes.permission;
            result.refreshtoken = usersession.code;
            result.accesstoken = GenerateJwtToken(new UserGenerateJwtTokenReq
            {
                userid = user.id,
                usermobile = user.mobile,
                useremail = user.email,
                username = user.name,
                permissionhex = PermissionToHex(user.attributes.permission)
            });

            return result;
        }
    }
}
