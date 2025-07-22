using PlanItNoww.Models;
using PlanItNoww.Utils;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;

namespace PlanItNoww.Services
{
    public class UsersService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;

        public UsersService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
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
                SELECT Users.id,Users.email                     ,Users.mobile                    ,Users.passwordhash              ,Users.googleid                  ,Users.roleid                    ,Users.pushnotificationtoken     ,Users.isemailverified           ,Users.ismobileverified          ,Users.isaadhaarverified         ,Users.version                   ,Users.notes                     ,Users.createdby               ,Users.createdon        ,Users.modifiedby           ,Users.modifiedon                ,Users.attributes                ,Users.isactive                  ,Users.issuspended 
                FROM Users
                ";
            var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
            if (req.id > 0)
            {
                queryBuilder.AddParameter("Users.id", "=", "id", req.id, DbTypes.Types.Long);
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
                    temp.email = reader["email                     "] == DBNull.Value ? "" : reader["email                     "].ToString();
                    temp.mobile = reader["mobile                    "] == DBNull.Value ? "" : reader["mobile                    "].ToString();
                    temp.passwordhash = reader["passwordhash              "] == DBNull.Value ? "" : reader["passwordhash              "].ToString();
                    temp.googleid = reader["googleid                  "] == DBNull.Value ? "" : reader["googleid                  "].ToString();
                    temp.roleid = reader["roleid                    "] == DBNull.Value ? 0 : Convert.ToInt64(reader["roleid                    "]);
                    temp.pushnotificationtoken = reader["pushnotificationtoken     "] == DBNull.Value ? "" : reader["pushnotificationtoken     "].ToString();
                    temp.isemailverified = reader["isemailverified           "] == DBNull.Value ? false : Convert.ToBoolean(reader["isemailverified           "]);
                    temp.ismobileverified = reader["ismobileverified          "] == DBNull.Value ? false : Convert.ToBoolean(reader["ismobileverified          "]);
                    temp.isaadhaarverified = reader["isaadhaarverified         "] == DBNull.Value ? false : Convert.ToBoolean(reader["isaadhaarverified         "]);
                    temp.version = reader["version                   "] == DBNull.Value ? 0 : Convert.ToInt32(reader["version                   "]);
                    temp.notes = reader["notes                     "] == DBNull.Value ? "" : reader["notes                     "].ToString();
                    temp.createdby = reader["createdby               "] == DBNull.Value ? 0 : Convert.ToInt64(reader["createdby               "]);
                    temp.createdon = reader["createdon        "] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["createdon        "]);
                    temp.modifiedby = reader["modifiedby           "] == DBNull.Value ? 0 : Convert.ToInt64(reader["modifiedby           "]);
                    temp.modifiedon = reader["modifiedon                "] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["modifiedon                "]);
                    temp.attributes_json = reader["attributes                "] == DBNull.Value ? "null" : reader["attributes                "].ToString();
                    temp.isactive = reader["isactive                  "] == DBNull.Value ? false : Convert.ToBoolean(reader["isactive                  "]);
                    temp.issuspended = reader["issuspended "] == DBNull.Value ? false : Convert.ToBoolean(reader["issuspended "]);
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
                    email                     ,mobile                    ,passwordhash              ,googleid                  ,roleid                    ,pushnotificationtoken     ,isemailverified           ,ismobileverified          ,isaadhaarverified         ,version                   ,notes                     ,createdby               ,createdon        ,modifiedby           ,modifiedon                ,attributes                ,isactive                  ,issuspended 
                )
                VALUES (
                   @email                     ,@mobile                    ,@passwordhash              ,@googleid                  ,@roleid                    ,@pushnotificationtoken     ,@isemailverified           ,@ismobileverified          ,@isaadhaarverified         ,@version                   ,@notes                     ,@createdby               ,@createdon        ,@modifiedby           ,@modifiedon                ,@attributes                ,@isactive                  ,@issuspended 
                )
                RETURNING id;
                ";
            users.isactive = true;
            users.version = 1;
            users.createdon = DateTime.UtcNow;
            users.createdby = requeststate.usercontext.id;
            users.modifiedon = DateTime.UtcNow;
            users.modifiedby = requeststate.usercontext.id;

            DbCommand command = db.GetCommand(query);

            db.AddParameter(command, "email                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.email) ? "" : users.email;
            db.AddParameter(command, "mobile                    ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.mobile) ? "" : users.mobile;
            db.AddParameter(command, "passwordhash              ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.passwordhash) ? "" : users.passwordhash;
            db.AddParameter(command, "googleid                  ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.googleid) ? "" : users.googleid;
            db.AddParameter(command, "roleid                    ", DbTypes.Types.Long).Value = users.roleid;
            db.AddParameter(command, "pushnotificationtoken     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.pushnotificationtoken) ? "" : users.pushnotificationtoken;
            db.AddParameter(command, "isemailverified           ", DbTypes.Types.Boolean).Value = users.isemailverified;
            db.AddParameter(command, "ismobileverified          ", DbTypes.Types.Boolean).Value = users.ismobileverified;
            db.AddParameter(command, "isaadhaarverified         ", DbTypes.Types.Boolean).Value = users.isaadhaarverified;
            db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = users.version;
            db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.notes) ? "" : users.notes;
            db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = users.createdby;
            db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = users.createdon;
            db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = users.modifiedby;
            db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = users.modifiedon;
            db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = users.attributes_json;
            db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = users.isactive;
            db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = users.issuspended;

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
                        email                      = @email                     ,mobile                     = @mobile                    ,passwordhash               = @passwordhash              ,googleid                   = @googleid                  ,roleid                     = @roleid                    ,pushnotificationtoken      = @pushnotificationtoken     ,isemailverified            = @isemailverified           ,ismobileverified           = @ismobileverified          ,isaadhaarverified          = @isaadhaarverified         ,version                    = @version                   ,notes                      = @notes                     ,createdby                = @createdby               ,createdon         = @createdon        ,modifiedby            = @modifiedby           ,modifiedon                 = @modifiedon                ,attributes                 = @attributes                ,isactive                   = @isactive                  ,issuspended  = @issuspended ,
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
            users.modifiedby = requeststate.usercontext.id;

            db.AddParameter(command, "id", DbTypes.Types.Long).Value = users.id;
            db.AddParameter(command, "email                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.email) ? "" : users.email;
            db.AddParameter(command, "mobile                    ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.mobile) ? "" : users.mobile;
            db.AddParameter(command, "passwordhash              ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.passwordhash) ? "" : users.passwordhash;
            db.AddParameter(command, "googleid                  ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.googleid) ? "" : users.googleid;
            db.AddParameter(command, "roleid                    ", DbTypes.Types.Long).Value = users.roleid;
            db.AddParameter(command, "pushnotificationtoken     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.pushnotificationtoken) ? "" : users.pushnotificationtoken;
            db.AddParameter(command, "isemailverified           ", DbTypes.Types.Boolean).Value = users.isemailverified;
            db.AddParameter(command, "ismobileverified          ", DbTypes.Types.Boolean).Value = users.ismobileverified;
            db.AddParameter(command, "isaadhaarverified         ", DbTypes.Types.Boolean).Value = users.isaadhaarverified;
            db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = users.version;
            db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(users.notes) ? "" : users.notes;
            db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = users.createdby;
            db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = users.createdon;
            db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = users.modifiedby;
            db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = users.modifiedon;
            db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = users.attributes_json;
            db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = users.isactive;
            db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = users.issuspended;

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
            db.AddParameter(command, "modifiedby", DbTypes.Types.Long).Value = requeststate.usercontext.id;
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
                var key = Encoding.ASCII.GetBytes(.jwtsecret);
            }
            catch
            {

            }
        }
    }
}
