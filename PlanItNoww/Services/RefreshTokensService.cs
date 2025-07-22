using PlanItNoww.Models;
using PlanItNoww.Utils;
using System.Data.Common;

namespace PlanItNoww.Services
{
    public class RefreshTokensService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;
        
        public RefreshTokensService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
        }
        public async Task<List<RefreshTokens>> Select(RefreshTokensSelectReq req)
        {
            List<RefreshTokens> result = null;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.SelectTransaction(db, req);
                }
            return result;
        }
        public async Task<List<RefreshTokens>> SelectTransaction(IDb db, RefreshTokensSelectReq req)
        {
            List<RefreshTokens> result = new List<RefreshTokens>();
                string query = @"
                SELECT RefreshTokens.id,RefreshTokens.token                     ,RefreshTokens.revoked                   ,RefreshTokens.userid,RefreshTokens.expiresat,RefreshTokens.version                   ,RefreshTokens.notes                     ,RefreshTokens.createdby               ,RefreshTokens.createdon        ,RefreshTokens.modifiedby           ,RefreshTokens.modifiedon                ,RefreshTokens.attributes                ,RefreshTokens.isactive                  ,RefreshTokens.issuspended 
                FROM RefreshTokens
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                if (req.id > 0)
                {
                    queryBuilder.AddParameter("RefreshTokens.id", "=", "id", req.id, DbTypes.Types.Long);
                }
                queryBuilder.AddParameter("RefreshTokens.isactive", "=", "isactive", true, DbTypes.Types.Boolean);

                queryBuilder.AddOrderBy(QueryBuilder.Order.ASC, "RefreshTokens.id");
                var command = queryBuilder.GetCommand(db);
                using (DbDataReader reader = await db.Execute(command))
                {
                    while (await reader.ReadAsync())
                    {
                        RefreshTokens temp = new RefreshTokens();
                         temp.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
temp.token                      = reader["token                     "] == DBNull.Value ? "" : reader["token                     "].ToString();
 temp.revoked                    = reader["revoked                   "] == DBNull.Value ? false : Convert.ToBoolean(reader["revoked                   "]);
temp.userid = reader["userid"] == DBNull.Value ? "" : reader["userid"].ToString();
temp.expiresat = reader["expiresat"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["expiresat"]);
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
        public async Task<RefreshTokens> Insert(RefreshTokens refreshtokens)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.InsertTransaction(db, refreshtokens);
                }
            return refreshtokens;
        }
        public async Task InsertTransaction(IDb db, RefreshTokens refreshtokens)
        {
                String query = @"
                INSERT INTO RefreshTokens (
                    token                     ,revoked                   ,userid,expiresat,version                   ,notes                     ,createdby               ,createdon        ,modifiedby           ,modifiedon                ,attributes                ,isactive                  ,issuspended 
                )
                VALUES (
                   @token                     ,@revoked                   ,@userid,@expiresat,@version                   ,@notes                     ,@createdby               ,@createdon        ,@modifiedby           ,@modifiedon                ,@attributes                ,@isactive                  ,@issuspended 
                )
                RETURNING id;
                ";
                refreshtokens.isactive = true;
                refreshtokens.version = 1;
                refreshtokens.createdon = DateTime.UtcNow;
                refreshtokens.createdby = requeststate.usercontext.id;
                refreshtokens.modifiedon = DateTime.UtcNow;
                refreshtokens.modifiedby = requeststate.usercontext.id;

                DbCommand command = db.GetCommand(query);

                db.AddParameter(command, "token                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(refreshtokens.token                     ) ? "" : refreshtokens.token                     ;
db.AddParameter(command, "revoked                   ", DbTypes.Types.Boolean).Value = refreshtokens.revoked                   ;
db.AddParameter(command, "userid", DbTypes.Types.String).Value = String.IsNullOrEmpty(refreshtokens.userid) ? "" : refreshtokens.userid;
db.AddParameter(command, "expiresat", DbTypes.Types.DateTime).Value = refreshtokens.expiresat;
db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = refreshtokens.version                   ;
db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(refreshtokens.notes                     ) ? "" : refreshtokens.notes                     ;
db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = refreshtokens.createdby               ;
db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = refreshtokens.createdon        ;
db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = refreshtokens.modifiedby           ;
db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = refreshtokens.modifiedon                ;
db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = refreshtokens.attributes                _json;
db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = refreshtokens.isactive                  ;
db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = refreshtokens.issuspended ;
                
                using (DbDataReader reader = await db.Execute(command))
                {
                    if (await reader.ReadAsync())
                    {
                        refreshtokens.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                    }
                }
            }
        public async Task<RefreshTokens> Update(RefreshTokens refreshtokens)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.UpdateTransaction(db, refreshtokens);
                }
            return refreshtokens;
        }
        public async Task<bool> UpdateTransaction(IDb db, RefreshTokens refreshtokens)
        {
            bool result = false;
                String query = @"
                UPDATE RefreshTokens
                    SET 
                        token                      = @token                     ,revoked                    = @revoked                   ,userid = @userid,expiresat = @expiresat,version                    = @version                   ,notes                      = @notes                     ,createdby                = @createdby               ,createdon         = @createdon        ,modifiedby            = @modifiedby           ,modifiedon                 = @modifiedon                ,attributes                 = @attributes                ,isactive                   = @isactive                  ,issuspended  = @issuspended ,
                        version = version + 1
                ";
                
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);

                queryBuilder.AddParameter("id", "=", "id", refreshtokens.id, DbTypes.Types.Long);

                if (refreshtokens.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", refreshtokens.version, DbTypes.Types.Integer);
                }

                var command = queryBuilder.GetCommand(db);
                
                refreshtokens.modifiedon = DateTime.UtcNow;
                refreshtokens.modifiedby = requeststate.usercontext.id;
                
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = refreshtokens.id;
db.AddParameter(command, "token                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(refreshtokens.token                     ) ? "" : refreshtokens.token                     ;
db.AddParameter(command, "revoked                   ", DbTypes.Types.Boolean).Value = refreshtokens.revoked                   ;
db.AddParameter(command, "userid", DbTypes.Types.String).Value = String.IsNullOrEmpty(refreshtokens.userid) ? "" : refreshtokens.userid;
db.AddParameter(command, "expiresat", DbTypes.Types.DateTime).Value = refreshtokens.expiresat;
db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = refreshtokens.version                   ;
db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(refreshtokens.notes                     ) ? "" : refreshtokens.notes                     ;
db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = refreshtokens.createdby               ;
db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = refreshtokens.createdon        ;
db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = refreshtokens.modifiedby           ;
db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = refreshtokens.modifiedon                ;
db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = refreshtokens.attributes                _json;
db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = refreshtokens.isactive                  ;
db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = refreshtokens.issuspended ;

                if (await db.ExecuteNonQuery(command) > 0)
                {
                    refreshtokens.version = refreshtokens.version + 1;
                    result = true;
                }
            return result;
        }
        public async Task<bool> Delete(RefreshTokensDeleteReq refreshtokens)
        {
             bool result = false;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.DeleteTransaction(db, refreshtokens);
                    
                }
            return result;
        }
        public async Task<bool> DeleteTransaction(IDb db, RefreshTokensDeleteReq refreshtokens)
        {
            bool result = false;
                String query = @"
                UPDATE RefreshTokens
                SET isactive = '0',
                    version = version + 1,
                    modifiedon = @modifiedon,
                    modifiedby = @modifiedby 
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                queryBuilder.AddParameter("id", "=", "id", refreshtokens.id, DbTypes.Types.Long);
                if (refreshtokens.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", refreshtokens.version, DbTypes.Types.Integer);
                }
                DbCommand command = queryBuilder.GetCommand(db);
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = refreshtokens.id;
                db.AddParameter(command, "version", DbTypes.Types.Integer).Value = refreshtokens.version;
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
