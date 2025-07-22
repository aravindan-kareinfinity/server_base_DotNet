using PlanItNoww.Models;
using PlanItNoww.Utils;
using System.Data.Common;

namespace PlanItNoww.Services
{
    public class OTPsService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;
        
        public OTPsService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
        }
        public async Task<List<OTPs>> Select(OTPsSelectReq req)
        {
            List<OTPs> result = null;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.SelectTransaction(db, req);
                }
            return result;
        }
        public async Task<List<OTPs>> SelectTransaction(IDb db, OTPsSelectReq req)
        {
            List<OTPs> result = new List<OTPs>();
                string query = @"
                SELECT OTPs.id,OTPs.userid,OTPs.emailormobile             ,OTPs.otpcode                   ,OTPs.purpose                   ,OTPs.expiresat                 ,OTPs.isused                   ,OTPs.version                   ,OTPs.notes                     ,OTPs.createdby               ,OTPs.createdon        ,OTPs.modifiedby           ,OTPs.modifiedon                ,OTPs.attributes                ,OTPs.isactive                  ,OTPs.issuspended 
                FROM OTPs
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                if (req.id > 0)
                {
                    queryBuilder.AddParameter("OTPs.id", "=", "id", req.id, DbTypes.Types.Long);
                }
                queryBuilder.AddParameter("OTPs.isactive", "=", "isactive", true, DbTypes.Types.Boolean);

                queryBuilder.AddOrderBy(QueryBuilder.Order.ASC, "OTPs.id");
                var command = queryBuilder.GetCommand(db);
                using (DbDataReader reader = await db.Execute(command))
                {
                    while (await reader.ReadAsync())
                    {
                        OTPs temp = new OTPs();
                         temp.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
temp.userid = reader["userid"] == DBNull.Value ? "" : reader["userid"].ToString();
temp.emailormobile              = reader["emailormobile             "] == DBNull.Value ? "" : reader["emailormobile             "].ToString();
temp.otpcode                    = reader["otpcode                   "] == DBNull.Value ? "" : reader["otpcode                   "].ToString();
temp.purpose                    = reader["purpose                   "] == DBNull.Value ? "" : reader["purpose                   "].ToString();
temp.expiresat                  = reader["expiresat                 "] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["expiresat                 "]);
 temp.isused                    = reader["isused                   "] == DBNull.Value ? false : Convert.ToBoolean(reader["isused                   "]);
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
        public async Task<OTPs> Insert(OTPs otps)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.InsertTransaction(db, otps);
                }
            return otps;
        }
        public async Task InsertTransaction(IDb db, OTPs otps)
        {
                String query = @"
                INSERT INTO OTPs (
                    userid,emailormobile             ,otpcode                   ,purpose                   ,expiresat                 ,isused                   ,version                   ,notes                     ,createdby               ,createdon        ,modifiedby           ,modifiedon                ,attributes                ,isactive                  ,issuspended 
                )
                VALUES (
                   @userid,@emailormobile             ,@otpcode                   ,@purpose                   ,@expiresat                 ,@isused                   ,@version                   ,@notes                     ,@createdby               ,@createdon        ,@modifiedby           ,@modifiedon                ,@attributes                ,@isactive                  ,@issuspended 
                )
                RETURNING id;
                ";
                otps.isactive = true;
                otps.version = 1;
                otps.createdon = DateTime.UtcNow;
                otps.createdby = requeststate.usercontext.id;
                otps.modifiedon = DateTime.UtcNow;
                otps.modifiedby = requeststate.usercontext.id;

                DbCommand command = db.GetCommand(query);

                db.AddParameter(command, "userid", DbTypes.Types.String).Value = String.IsNullOrEmpty(otps.userid) ? "" : otps.userid;
db.AddParameter(command, "emailormobile             ", DbTypes.Types.String).Value = String.IsNullOrEmpty(otps.emailormobile             ) ? "" : otps.emailormobile             ;
db.AddParameter(command, "otpcode                   ", DbTypes.Types.String).Value = String.IsNullOrEmpty(otps.otpcode                   ) ? "" : otps.otpcode                   ;
db.AddParameter(command, "purpose                   ", DbTypes.Types.String).Value = String.IsNullOrEmpty(otps.purpose                   ) ? "" : otps.purpose                   ;
db.AddParameter(command, "expiresat                 ", DbTypes.Types.DateTime).Value = otps.expiresat                 ;
db.AddParameter(command, "isused                   ", DbTypes.Types.Boolean).Value = otps.isused                   ;
db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = otps.version                   ;
db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(otps.notes                     ) ? "" : otps.notes                     ;
db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = otps.createdby               ;
db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = otps.createdon        ;
db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = otps.modifiedby           ;
db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = otps.modifiedon                ;
db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = otps.attributes                _json;
db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = otps.isactive                  ;
db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = otps.issuspended ;
                
                using (DbDataReader reader = await db.Execute(command))
                {
                    if (await reader.ReadAsync())
                    {
                        otps.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                    }
                }
            }
        public async Task<OTPs> Update(OTPs otps)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.UpdateTransaction(db, otps);
                }
            return otps;
        }
        public async Task<bool> UpdateTransaction(IDb db, OTPs otps)
        {
            bool result = false;
                String query = @"
                UPDATE OTPs
                    SET 
                        userid = @userid,emailormobile              = @emailormobile             ,otpcode                    = @otpcode                   ,purpose                    = @purpose                   ,expiresat                  = @expiresat                 ,isused                    = @isused                   ,version                    = @version                   ,notes                      = @notes                     ,createdby                = @createdby               ,createdon         = @createdon        ,modifiedby            = @modifiedby           ,modifiedon                 = @modifiedon                ,attributes                 = @attributes                ,isactive                   = @isactive                  ,issuspended  = @issuspended ,
                        version = version + 1
                ";
                
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);

                queryBuilder.AddParameter("id", "=", "id", otps.id, DbTypes.Types.Long);

                if (otps.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", otps.version, DbTypes.Types.Integer);
                }

                var command = queryBuilder.GetCommand(db);
                
                otps.modifiedon = DateTime.UtcNow;
                otps.modifiedby = requeststate.usercontext.id;
                
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = otps.id;
db.AddParameter(command, "userid", DbTypes.Types.String).Value = String.IsNullOrEmpty(otps.userid) ? "" : otps.userid;
db.AddParameter(command, "emailormobile             ", DbTypes.Types.String).Value = String.IsNullOrEmpty(otps.emailormobile             ) ? "" : otps.emailormobile             ;
db.AddParameter(command, "otpcode                   ", DbTypes.Types.String).Value = String.IsNullOrEmpty(otps.otpcode                   ) ? "" : otps.otpcode                   ;
db.AddParameter(command, "purpose                   ", DbTypes.Types.String).Value = String.IsNullOrEmpty(otps.purpose                   ) ? "" : otps.purpose                   ;
db.AddParameter(command, "expiresat                 ", DbTypes.Types.DateTime).Value = otps.expiresat                 ;
db.AddParameter(command, "isused                   ", DbTypes.Types.Boolean).Value = otps.isused                   ;
db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = otps.version                   ;
db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(otps.notes                     ) ? "" : otps.notes                     ;
db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = otps.createdby               ;
db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = otps.createdon        ;
db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = otps.modifiedby           ;
db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = otps.modifiedon                ;
db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = otps.attributes                _json;
db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = otps.isactive                  ;
db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = otps.issuspended ;

                if (await db.ExecuteNonQuery(command) > 0)
                {
                    otps.version = otps.version + 1;
                    result = true;
                }
            return result;
        }
        public async Task<bool> Delete(OTPsDeleteReq otps)
        {
             bool result = false;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.DeleteTransaction(db, otps);
                    
                }
            return result;
        }
        public async Task<bool> DeleteTransaction(IDb db, OTPsDeleteReq otps)
        {
            bool result = false;
                String query = @"
                UPDATE OTPs
                SET isactive = '0',
                    version = version + 1,
                    modifiedon = @modifiedon,
                    modifiedby = @modifiedby 
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                queryBuilder.AddParameter("id", "=", "id", otps.id, DbTypes.Types.Long);
                if (otps.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", otps.version, DbTypes.Types.Integer);
                }
                DbCommand command = queryBuilder.GetCommand(db);
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = otps.id;
                db.AddParameter(command, "version", DbTypes.Types.Integer).Value = otps.version;
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
