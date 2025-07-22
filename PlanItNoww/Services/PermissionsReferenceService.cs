using PlanItNoww.Models;
using PlanItNoww.Utils;
using System.Data.Common;

namespace PlanItNoww.Services
{
    public class PermissionsReferenceService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;
        
        public PermissionsReferenceService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
        }
        public async Task<List<PermissionsReference>> Select(PermissionsReferenceSelectReq req)
        {
            List<PermissionsReference> result = null;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.SelectTransaction(db, req);
                }
            return result;
        }
        public async Task<List<PermissionsReference>> SelectTransaction(IDb db, PermissionsReferenceSelectReq req)
        {
            List<PermissionsReference> result = new List<PermissionsReference>();
                string query = @"
                SELECT PermissionsReference.id,PermissionsReference.bitposition               ,PermissionsReference.value                     ,PermissionsReference.description               ,PermissionsReference.name,PermissionsReference.version                   ,PermissionsReference.notes                     ,PermissionsReference.createdby               ,PermissionsReference.createdon        ,PermissionsReference.modifiedby           ,PermissionsReference.modifiedon                ,PermissionsReference.attributes                ,PermissionsReference.isactive                  ,PermissionsReference.issuspended 
                FROM PermissionsReference
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                if (req.id > 0)
                {
                    queryBuilder.AddParameter("PermissionsReference.id", "=", "id", req.id, DbTypes.Types.Long);
                }
                queryBuilder.AddParameter("PermissionsReference.isactive", "=", "isactive", true, DbTypes.Types.Boolean);

                queryBuilder.AddOrderBy(QueryBuilder.Order.ASC, "PermissionsReference.id");
                var command = queryBuilder.GetCommand(db);
                using (DbDataReader reader = await db.Execute(command))
                {
                    while (await reader.ReadAsync())
                    {
                        PermissionsReference temp = new PermissionsReference();
                         temp.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
 temp.bitposition                = reader["bitposition               "] == DBNull.Value ? 0 : Convert.ToInt32(reader["bitposition               "]);
 temp.value                      = reader["value                     "] == DBNull.Value ? 0 : Convert.ToInt32(reader["value                     "]);
temp.description                = reader["description               "] == DBNull.Value ? "" : reader["description               "].ToString();
temp.name = reader["name"] == DBNull.Value ? "" : reader["name"].ToString();
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
        public async Task<PermissionsReference> Insert(PermissionsReference permissionsreference)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.InsertTransaction(db, permissionsreference);
                }
            return permissionsreference;
        }
        public async Task InsertTransaction(IDb db, PermissionsReference permissionsreference)
        {
                String query = @"
                INSERT INTO PermissionsReference (
                    bitposition               ,value                     ,description               ,name,version                   ,notes                     ,createdby               ,createdon        ,modifiedby           ,modifiedon                ,attributes                ,isactive                  ,issuspended 
                )
                VALUES (
                   @bitposition               ,@value                     ,@description               ,@name,@version                   ,@notes                     ,@createdby               ,@createdon        ,@modifiedby           ,@modifiedon                ,@attributes                ,@isactive                  ,@issuspended 
                )
                RETURNING id;
                ";
                permissionsreference.isactive = true;
                permissionsreference.version = 1;
                permissionsreference.createdon = DateTime.UtcNow;
                permissionsreference.createdby = requeststate.usercontext.id;
                permissionsreference.modifiedon = DateTime.UtcNow;
                permissionsreference.modifiedby = requeststate.usercontext.id;

                DbCommand command = db.GetCommand(query);

                db.AddParameter(command, "bitposition               ", DbTypes.Types.Integer).Value = permissionsreference.bitposition               ;
db.AddParameter(command, "value                     ", DbTypes.Types.Integer).Value = permissionsreference.value                     ;
db.AddParameter(command, "description               ", DbTypes.Types.String).Value = String.IsNullOrEmpty(permissionsreference.description               ) ? "" : permissionsreference.description               ;
db.AddParameter(command, "name", DbTypes.Types.String).Value = String.IsNullOrEmpty(permissionsreference.name) ? "" : permissionsreference.name;
db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = permissionsreference.version                   ;
db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(permissionsreference.notes                     ) ? "" : permissionsreference.notes                     ;
db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = permissionsreference.createdby               ;
db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = permissionsreference.createdon        ;
db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = permissionsreference.modifiedby           ;
db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = permissionsreference.modifiedon                ;
db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = permissionsreference.attributes                _json;
db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = permissionsreference.isactive                  ;
db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = permissionsreference.issuspended ;
                
                using (DbDataReader reader = await db.Execute(command))
                {
                    if (await reader.ReadAsync())
                    {
                        permissionsreference.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                    }
                }
            }
        public async Task<PermissionsReference> Update(PermissionsReference permissionsreference)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.UpdateTransaction(db, permissionsreference);
                }
            return permissionsreference;
        }
        public async Task<bool> UpdateTransaction(IDb db, PermissionsReference permissionsreference)
        {
            bool result = false;
                String query = @"
                UPDATE PermissionsReference
                    SET 
                        bitposition                = @bitposition               ,value                      = @value                     ,description                = @description               ,name = @name,version                    = @version                   ,notes                      = @notes                     ,createdby                = @createdby               ,createdon         = @createdon        ,modifiedby            = @modifiedby           ,modifiedon                 = @modifiedon                ,attributes                 = @attributes                ,isactive                   = @isactive                  ,issuspended  = @issuspended ,
                        version = version + 1
                ";
                
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);

                queryBuilder.AddParameter("id", "=", "id", permissionsreference.id, DbTypes.Types.Long);

                if (permissionsreference.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", permissionsreference.version, DbTypes.Types.Integer);
                }

                var command = queryBuilder.GetCommand(db);
                
                permissionsreference.modifiedon = DateTime.UtcNow;
                permissionsreference.modifiedby = requeststate.usercontext.id;
                
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = permissionsreference.id;
db.AddParameter(command, "bitposition               ", DbTypes.Types.Integer).Value = permissionsreference.bitposition               ;
db.AddParameter(command, "value                     ", DbTypes.Types.Integer).Value = permissionsreference.value                     ;
db.AddParameter(command, "description               ", DbTypes.Types.String).Value = String.IsNullOrEmpty(permissionsreference.description               ) ? "" : permissionsreference.description               ;
db.AddParameter(command, "name", DbTypes.Types.String).Value = String.IsNullOrEmpty(permissionsreference.name) ? "" : permissionsreference.name;
db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = permissionsreference.version                   ;
db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(permissionsreference.notes                     ) ? "" : permissionsreference.notes                     ;
db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = permissionsreference.createdby               ;
db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = permissionsreference.createdon        ;
db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = permissionsreference.modifiedby           ;
db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = permissionsreference.modifiedon                ;
db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = permissionsreference.attributes                _json;
db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = permissionsreference.isactive                  ;
db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = permissionsreference.issuspended ;

                if (await db.ExecuteNonQuery(command) > 0)
                {
                    permissionsreference.version = permissionsreference.version + 1;
                    result = true;
                }
            return result;
        }
        public async Task<bool> Delete(PermissionsReferenceDeleteReq permissionsreference)
        {
             bool result = false;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.DeleteTransaction(db, permissionsreference);
                    
                }
            return result;
        }
        public async Task<bool> DeleteTransaction(IDb db, PermissionsReferenceDeleteReq permissionsreference)
        {
            bool result = false;
                String query = @"
                UPDATE PermissionsReference
                SET isactive = '0',
                    version = version + 1,
                    modifiedon = @modifiedon,
                    modifiedby = @modifiedby 
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                queryBuilder.AddParameter("id", "=", "id", permissionsreference.id, DbTypes.Types.Long);
                if (permissionsreference.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", permissionsreference.version, DbTypes.Types.Integer);
                }
                DbCommand command = queryBuilder.GetCommand(db);
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = permissionsreference.id;
                db.AddParameter(command, "version", DbTypes.Types.Integer).Value = permissionsreference.version;
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
