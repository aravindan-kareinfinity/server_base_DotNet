using PlanItNoww.Models;
using PlanItNoww.Utils;
using System.Data.Common;

namespace PlanItNoww.Services
{
    public class UserProfilesService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;
        
        public UserProfilesService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
        }
        public async Task<List<UserProfiles>> Select(UserProfilesSelectReq req)
        {
            List<UserProfiles> result = null;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.SelectTransaction(db, req);
                }
            return result;
        }
        public async Task<List<UserProfiles>> SelectTransaction(IDb db, UserProfilesSelectReq req)
        {
            List<UserProfiles> result = new List<UserProfiles>();
                string query = @"
                SELECT UserProfiles.id,UserProfiles.userid,UserProfiles.fullname,UserProfiles.dob,UserProfiles.gender,UserProfiles.address,UserProfiles.city,UserProfiles.state,UserProfiles.zipcode,UserProfiles.country,UserProfiles.profileimageurl,UserProfiles.version                   ,UserProfiles.notes                     ,UserProfiles.createdby               ,UserProfiles.createdon        ,UserProfiles.modifiedby           ,UserProfiles.modifiedon                ,UserProfiles.attributes                ,UserProfiles.isactive                  ,UserProfiles.issuspended 
                FROM UserProfiles
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                if (req.id > 0)
                {
                    queryBuilder.AddParameter("UserProfiles.id", "=", "id", req.id, DbTypes.Types.Long);
                }
                queryBuilder.AddParameter("UserProfiles.isactive", "=", "isactive", true, DbTypes.Types.Boolean);

                queryBuilder.AddOrderBy(QueryBuilder.Order.ASC, "UserProfiles.id");
                var command = queryBuilder.GetCommand(db);
                using (DbDataReader reader = await db.Execute(command))
                {
                    while (await reader.ReadAsync())
                    {
                        UserProfiles temp = new UserProfiles();
                         temp.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
temp.userid = reader["userid"] == DBNull.Value ? "" : reader["userid"].ToString();
temp.fullname = reader["fullname"] == DBNull.Value ? "" : reader["fullname"].ToString();
temp.dob = reader["dob"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["dob"]);
temp.gender = reader["gender"] == DBNull.Value ? "" : reader["gender"].ToString();
temp.address = reader["address"] == DBNull.Value ? "" : reader["address"].ToString();
temp.city = reader["city"] == DBNull.Value ? "" : reader["city"].ToString();
temp.state = reader["state"] == DBNull.Value ? "" : reader["state"].ToString();
temp.zipcode = reader["zipcode"] == DBNull.Value ? "" : reader["zipcode"].ToString();
temp.country = reader["country"] == DBNull.Value ? "" : reader["country"].ToString();
temp.profileimageurl = reader["profileimageurl"] == DBNull.Value ? "" : reader["profileimageurl"].ToString();
 temp.version                    = reader["version                   "] == DBNull.Value ? 0 : Convert.ToInt32(reader["version                   "]);
temp.notes                      = reader["notes                     "] == DBNull.Value ? "" : reader["notes                     "].ToString();
 temp.createdby                = reader["createdby               "] == DBNull.Value ? 0 : Convert.ToInt64(reader["createdby               "]);
temp.createdon         = reader["createdon        "] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["createdon        "]);
 temp.modifiedby            = reader["modifiedby           "] == DBNull.Value ? 0 : Convert.ToInt64(reader["modifiedby           "]);
temp.modifiedon                 = reader["modifiedon                "] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["modifiedon                "]);
temp.attributes                _json = reader["attributes                "] == DBNull.Value ? "null" : reader["attributes                "].ToString();
 temp.isactive                   = reader["isactive                  "] == DBNull.Value ? false : Convert.ToBoolean(reader["isactive                  "]);
 temp.issuspended  = reader["issuspended "] == DBNull.Value ? false : Convert.ToBoolean(reader["issuspended "]);
                        result.Add(temp);
                    }
                }
            return result;
        }
        public async Task<UserProfiles> Insert(UserProfiles userprofiles)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.InsertTransaction(db, userprofiles);
                }
            return userprofiles;
        }
        public async Task InsertTransaction(IDb db, UserProfiles userprofiles)
        {
                String query = @"
                INSERT INTO UserProfiles (
                    userid,fullname,dob,gender,address,city,state,zipcode,country,profileimageurl,version                   ,notes                     ,createdby               ,createdon        ,modifiedby           ,modifiedon                ,attributes                ,isactive                  ,issuspended 
                )
                VALUES (
                   @userid,@fullname,@dob,@gender,@address,@city,@state,@zipcode,@country,@profileimageurl,@version                   ,@notes                     ,@createdby               ,@createdon        ,@modifiedby           ,@modifiedon                ,@attributes                ,@isactive                  ,@issuspended 
                )
                RETURNING id;
                ";
                userprofiles.isactive = true;
                userprofiles.version = 1;
                userprofiles.createdon = DateTime.UtcNow;
                userprofiles.createdby = requeststate.usercontext.id;
                userprofiles.modifiedon = DateTime.UtcNow;
                userprofiles.modifiedby = requeststate.usercontext.id;

                DbCommand command = db.GetCommand(query);

                db.AddParameter(command, "userid", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.userid) ? "" : userprofiles.userid;
db.AddParameter(command, "fullname", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.fullname) ? "" : userprofiles.fullname;
db.AddParameter(command, "dob", DbTypes.Types.DateTime).Value = userprofiles.dob;
db.AddParameter(command, "gender", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.gender) ? "" : userprofiles.gender;
db.AddParameter(command, "address", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.address) ? "" : userprofiles.address;
db.AddParameter(command, "city", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.city) ? "" : userprofiles.city;
db.AddParameter(command, "state", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.state) ? "" : userprofiles.state;
db.AddParameter(command, "zipcode", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.zipcode) ? "" : userprofiles.zipcode;
db.AddParameter(command, "country", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.country) ? "" : userprofiles.country;
db.AddParameter(command, "profileimageurl", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.profileimageurl) ? "" : userprofiles.profileimageurl;
db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = userprofiles.version                   ;
db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.notes                     ) ? "" : userprofiles.notes                     ;
db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = userprofiles.createdby               ;
db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = userprofiles.createdon        ;
db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = userprofiles.modifiedby           ;
db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = userprofiles.modifiedon                ;
db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = userprofiles.attributes                _json;
db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = userprofiles.isactive                  ;
db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = userprofiles.issuspended ;
                
                using (DbDataReader reader = await db.Execute(command))
                {
                    if (await reader.ReadAsync())
                    {
                        userprofiles.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                    }
                }
            }
        public async Task<UserProfiles> Update(UserProfiles userprofiles)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.UpdateTransaction(db, userprofiles);
                }
            return userprofiles;
        }
        public async Task<bool> UpdateTransaction(IDb db, UserProfiles userprofiles)
        {
            bool result = false;
                String query = @"
                UPDATE UserProfiles
                    SET 
                        userid = @userid,fullname = @fullname,dob = @dob,gender = @gender,address = @address,city = @city,state = @state,zipcode = @zipcode,country = @country,profileimageurl = @profileimageurl,version                    = @version                   ,notes                      = @notes                     ,createdby                = @createdby               ,createdon         = @createdon        ,modifiedby            = @modifiedby           ,modifiedon                 = @modifiedon                ,attributes                 = @attributes                ,isactive                   = @isactive                  ,issuspended  = @issuspended ,
                        version = version + 1
                ";
                
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);

                queryBuilder.AddParameter("id", "=", "id", userprofiles.id, DbTypes.Types.Long);

                if (userprofiles.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", userprofiles.version, DbTypes.Types.Integer);
                }

                var command = queryBuilder.GetCommand(db);
                
                userprofiles.modifiedon = DateTime.UtcNow;
                userprofiles.modifiedby = requeststate.usercontext.id;
                
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = userprofiles.id;
db.AddParameter(command, "userid", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.userid) ? "" : userprofiles.userid;
db.AddParameter(command, "fullname", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.fullname) ? "" : userprofiles.fullname;
db.AddParameter(command, "dob", DbTypes.Types.DateTime).Value = userprofiles.dob;
db.AddParameter(command, "gender", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.gender) ? "" : userprofiles.gender;
db.AddParameter(command, "address", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.address) ? "" : userprofiles.address;
db.AddParameter(command, "city", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.city) ? "" : userprofiles.city;
db.AddParameter(command, "state", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.state) ? "" : userprofiles.state;
db.AddParameter(command, "zipcode", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.zipcode) ? "" : userprofiles.zipcode;
db.AddParameter(command, "country", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.country) ? "" : userprofiles.country;
db.AddParameter(command, "profileimageurl", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.profileimageurl) ? "" : userprofiles.profileimageurl;
db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = userprofiles.version                   ;
db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(userprofiles.notes                     ) ? "" : userprofiles.notes                     ;
db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = userprofiles.createdby               ;
db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = userprofiles.createdon        ;
db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = userprofiles.modifiedby           ;
db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = userprofiles.modifiedon                ;
db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = userprofiles.attributes                _json;
db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = userprofiles.isactive                  ;
db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = userprofiles.issuspended ;

                if (await db.ExecuteNonQuery(command) > 0)
                {
                    userprofiles.version = userprofiles.version + 1;
                    result = true;
                }
            return result;
        }
        public async Task<bool> Delete(UserProfilesDeleteReq userprofiles)
        {
             bool result = false;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.DeleteTransaction(db, userprofiles);
                    
                }
            return result;
        }
        public async Task<bool> DeleteTransaction(IDb db, UserProfilesDeleteReq userprofiles)
        {
            bool result = false;
                String query = @"
                UPDATE UserProfiles
                SET isactive = '0',
                    version = version + 1,
                    modifiedon = @modifiedon,
                    modifiedby = @modifiedby 
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                queryBuilder.AddParameter("id", "=", "id", userprofiles.id, DbTypes.Types.Long);
                if (userprofiles.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", userprofiles.version, DbTypes.Types.Integer);
                }
                DbCommand command = queryBuilder.GetCommand(db);
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = userprofiles.id;
                db.AddParameter(command, "version", DbTypes.Types.Integer).Value = userprofiles.version;
                db.AddParameter(command, "modifiedby", DbTypes.Types.Long).Value = requeststate.usercontext.id;
                db.AddParameter(command, "modifiedon", DbTypes.Types.DateTime).Value = DateTime.UtcNow;
                if (await db.ExecuteNonQuery(command) > 0)
                {
                    result = true;
                }
            return result;
        }
    }
}
