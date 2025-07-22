using PlanItNoww.Models;
using PlanItNoww.Utils;
using System.Data.Common;

namespace PlanItNoww.Services
{
    public class AadhaarVerificationsService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;
        
        public AadhaarVerificationsService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
        }
        public async Task<List<AadhaarVerifications>> Select(AadhaarVerificationsSelectReq req)
        {
            List<AadhaarVerifications> result = null;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.SelectTransaction(db, req);
                }
            return result;
        }
        public async Task<List<AadhaarVerifications>> SelectTransaction(IDb db, AadhaarVerificationsSelectReq req)
        {
            List<AadhaarVerifications> result = new List<AadhaarVerifications>();
                string query = @"
                SELECT AadhaarVerifications.id,AadhaarVerifications.userid,AadhaarVerifications.aadhaarnumbermasked,AadhaarVerifications.verificationstatus,AadhaarVerifications.verificationsource,AadhaarVerifications.verifiedat,AadhaarVerifications.responsedata,AadhaarVerifications.createdby,AadhaarVerifications.createdon,AadhaarVerifications.modifiedby,AadhaarVerifications.modifiedon,AadhaarVerifications.version,AadhaarVerifications.isactive,AadhaarVerifications.issuspended
                FROM AadhaarVerifications
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                if (req.id > 0)
                {
                    queryBuilder.AddParameter("AadhaarVerifications.id", "=", "id", req.id, DbTypes.Types.Long);
                }
                queryBuilder.AddParameter("AadhaarVerifications.isactive", "=", "isactive", true, DbTypes.Types.Boolean);

                queryBuilder.AddOrderBy(QueryBuilder.Order.ASC, "AadhaarVerifications.id");
                var command = queryBuilder.GetCommand(db);
                using (DbDataReader reader = await db.Execute(command))
                {
                    while (await reader.ReadAsync())
                    {
                        AadhaarVerifications temp = new AadhaarVerifications();
                         temp.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
 temp.userid = reader["userid"] == DBNull.Value ? 0 : Convert.ToInt64(reader["userid"]);
temp.aadhaarnumbermasked = reader["aadhaarnumbermasked"] == DBNull.Value ? "" : reader["aadhaarnumbermasked"].ToString();
temp.verificationstatus = reader["verificationstatus"] == DBNull.Value ? "" : reader["verificationstatus"].ToString();
temp.verificationsource = reader["verificationsource"] == DBNull.Value ? "" : reader["verificationsource"].ToString();
temp.verifiedat = reader["verifiedat"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["verifiedat"]);
temp.responsedata_json = reader["responsedata"] == DBNull.Value ? "null" : reader["responsedata"].ToString();
 temp.createdby = reader["createdby"] == DBNull.Value ? 0 : Convert.ToInt64(reader["createdby"]);
temp.createdon = reader["createdon"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["createdon"]);
 temp.modifiedby = reader["modifiedby"] == DBNull.Value ? 0 : Convert.ToInt64(reader["modifiedby"]);
temp.modifiedon = reader["modifiedon"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["modifiedon"]);
 temp.version = reader["version"] == DBNull.Value ? 0 : Convert.ToInt32(reader["version"]);
 temp.isactive = reader["isactive"] == DBNull.Value ? false : Convert.ToBoolean(reader["isactive"]);
 temp.issuspended = reader["issuspended"] == DBNull.Value ? false : Convert.ToBoolean(reader["issuspended"]);
                        result.Add(temp);
                    }
                }
            return result;
        }
        public async Task<AadhaarVerifications> Insert(AadhaarVerifications aadhaarverifications)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.InsertTransaction(db, aadhaarverifications);
                }
            return aadhaarverifications;
        }
        public async Task InsertTransaction(IDb db, AadhaarVerifications aadhaarverifications)
        {
                String query = @"
                INSERT INTO AadhaarVerifications (
                    userid,aadhaarnumbermasked,verificationstatus,verificationsource,verifiedat,responsedata,createdby,createdon,modifiedby,modifiedon,version,isactive,issuspended
                )
                VALUES (
                   @userid,@aadhaarnumbermasked,@verificationstatus,@verificationsource,@verifiedat,@responsedata,@createdby,@createdon,@modifiedby,@modifiedon,@version,@isactive,@issuspended
                )
                RETURNING id;
                ";
                aadhaarverifications.isactive = true;
                aadhaarverifications.version = 1;
                aadhaarverifications.createdon = DateTime.UtcNow;
                aadhaarverifications.createdby = requeststate.usercontext.id;
                aadhaarverifications.modifiedon = DateTime.UtcNow;
                aadhaarverifications.modifiedby = requeststate.usercontext.id;

                DbCommand command = db.GetCommand(query);

                db.AddParameter(command, "userid", DbTypes.Types.Long).Value = aadhaarverifications.userid;
db.AddParameter(command, "aadhaarnumbermasked", DbTypes.Types.String).Value = String.IsNullOrEmpty(aadhaarverifications.aadhaarnumbermasked) ? "" : aadhaarverifications.aadhaarnumbermasked;
db.AddParameter(command, "verificationstatus", DbTypes.Types.String).Value = String.IsNullOrEmpty(aadhaarverifications.verificationstatus) ? "" : aadhaarverifications.verificationstatus;
db.AddParameter(command, "verificationsource", DbTypes.Types.String).Value = String.IsNullOrEmpty(aadhaarverifications.verificationsource) ? "" : aadhaarverifications.verificationsource;
db.AddParameter(command, "verifiedat", DbTypes.Types.DateTime).Value = aadhaarverifications.verifiedat;
db.AddParameter(command, "responsedata", DbTypes.Types.Json).Value = aadhaarverifications.responsedata_json;
db.AddParameter(command, "createdby", DbTypes.Types.Long).Value = aadhaarverifications.createdby;
db.AddParameter(command, "createdon", DbTypes.Types.DateTime).Value = aadhaarverifications.createdon;
db.AddParameter(command, "modifiedby", DbTypes.Types.Long).Value = aadhaarverifications.modifiedby;
db.AddParameter(command, "modifiedon", DbTypes.Types.DateTime).Value = aadhaarverifications.modifiedon;
db.AddParameter(command, "version", DbTypes.Types.Integer).Value = aadhaarverifications.version;
db.AddParameter(command, "isactive", DbTypes.Types.Boolean).Value = aadhaarverifications.isactive;
db.AddParameter(command, "issuspended", DbTypes.Types.Boolean).Value = aadhaarverifications.issuspended;
                
                using (DbDataReader reader = await db.Execute(command))
                {
                    if (await reader.ReadAsync())
                    {
                        aadhaarverifications.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                    }
                }
            }
        public async Task<AadhaarVerifications> Update(AadhaarVerifications aadhaarverifications)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.UpdateTransaction(db, aadhaarverifications);
                }
            return aadhaarverifications;
        }
        public async Task<bool> UpdateTransaction(IDb db, AadhaarVerifications aadhaarverifications)
        {
            bool result = false;
                String query = @"
                UPDATE AadhaarVerifications
                    SET 
                        userid = @userid,aadhaarnumbermasked = @aadhaarnumbermasked,verificationstatus = @verificationstatus,verificationsource = @verificationsource,verifiedat = @verifiedat,responsedata = @responsedata,modifiedby = @modifiedby,modifiedon = @modifiedon,issuspended = @issuspended,
                        version = version + 1
                ";
                
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);

                queryBuilder.AddParameter("id", "=", "id", aadhaarverifications.id, DbTypes.Types.Long);

                if (aadhaarverifications.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", aadhaarverifications.version, DbTypes.Types.Integer);
                }

                var command = queryBuilder.GetCommand(db);
                
                aadhaarverifications.modifiedon = DateTime.UtcNow;
                aadhaarverifications.modifiedby = requeststate.usercontext.id;
                
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = aadhaarverifications.id;
db.AddParameter(command, "userid", DbTypes.Types.Long).Value = aadhaarverifications.userid;
db.AddParameter(command, "aadhaarnumbermasked", DbTypes.Types.String).Value = String.IsNullOrEmpty(aadhaarverifications.aadhaarnumbermasked) ? "" : aadhaarverifications.aadhaarnumbermasked;
db.AddParameter(command, "verificationstatus", DbTypes.Types.String).Value = String.IsNullOrEmpty(aadhaarverifications.verificationstatus) ? "" : aadhaarverifications.verificationstatus;
db.AddParameter(command, "verificationsource", DbTypes.Types.String).Value = String.IsNullOrEmpty(aadhaarverifications.verificationsource) ? "" : aadhaarverifications.verificationsource;
db.AddParameter(command, "verifiedat", DbTypes.Types.DateTime).Value = aadhaarverifications.verifiedat;
db.AddParameter(command, "responsedata", DbTypes.Types.Json).Value = aadhaarverifications.responsedata_json;
db.AddParameter(command, "modifiedby", DbTypes.Types.Long).Value = aadhaarverifications.modifiedby;
db.AddParameter(command, "modifiedon", DbTypes.Types.DateTime).Value = aadhaarverifications.modifiedon;
db.AddParameter(command, "version", DbTypes.Types.Integer).Value = aadhaarverifications.version;
db.AddParameter(command, "issuspended", DbTypes.Types.Boolean).Value = aadhaarverifications.issuspended;

                if (await db.ExecuteNonQuery(command) > 0)
                {
                    aadhaarverifications.version = aadhaarverifications.version + 1;
                    result = true;
                }
            return result;
        }
        public async Task<bool> Delete(AadhaarVerificationsDeleteReq aadhaarverifications)
        {
             bool result = false;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.DeleteTransaction(db, aadhaarverifications);
                    
                }
            return result;
        }
        public async Task<bool> DeleteTransaction(IDb db, AadhaarVerificationsDeleteReq aadhaarverifications)
        {
            bool result = false;
                String query = @"
                UPDATE AadhaarVerifications
                SET isactive = '0',
                    version = version + 1,
                    modifiedon = @modifiedon,
                    modifiedby = @modifiedby 
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                queryBuilder.AddParameter("id", "=", "id", aadhaarverifications.id, DbTypes.Types.Long);
                if (aadhaarverifications.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", aadhaarverifications.version, DbTypes.Types.Integer);
                }
                DbCommand command = queryBuilder.GetCommand(db);
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = aadhaarverifications.id;
                db.AddParameter(command, "version", DbTypes.Types.Integer).Value = aadhaarverifications.version;
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
