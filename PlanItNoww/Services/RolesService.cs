using PlanItNoww.Models;
using PlanItNoww.Utils;
using System.Data.Common;

namespace PlanItNoww.Services
{
    public class RolesService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;
        
        public RolesService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
        }
        public async Task<List<Roles>> Select(RolesSelectReq req)
        {
            List<Roles> result = null;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.SelectTransaction(db, req);
                }
            return result;
        }
        public async Task<List<Roles>> SelectTransaction(IDb db, RolesSelectReq req)
        {
            List<Roles> result = new List<Roles>();
                string query = @"
                SELECT Roles.id,Roles.name                      ,Roles.bitmask                   ,Roles.version                   ,Roles.notes                     ,Roles.createdby               ,Roles.createdon        ,Roles.modifiedby           ,Roles.modifiedon                ,Roles.attributes                ,Roles.isactive                  ,Roles.issuspended 
                FROM Roles
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                if (req.id > 0)
                {
                    queryBuilder.AddParameter("Roles.id", "=", "id", req.id, DbTypes.Types.Long);
                }
                queryBuilder.AddParameter("Roles.isactive", "=", "isactive", true, DbTypes.Types.Boolean);

                queryBuilder.AddOrderBy(QueryBuilder.Order.ASC, "Roles.id");
                var command = queryBuilder.GetCommand(db);
                using (DbDataReader reader = await db.Execute(command))
                {
                    while (await reader.ReadAsync())
                    {
                        Roles temp = new Roles();
                         temp.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
temp.name                       = reader["name                      "] == DBNull.Value ? "" : reader["name                      "].ToString();
 temp.bitmask                    = reader["bitmask                   "] == DBNull.Value ? 0 : Convert.ToInt32(reader["bitmask                   "]);
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
        public async Task<Roles> Insert(Roles roles)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.InsertTransaction(db, roles);
                }
            return roles;
        }
        public async Task InsertTransaction(IDb db, Roles roles)
        {
                String query = @"
                INSERT INTO Roles (
                    name                      ,bitmask                   ,version                   ,notes                     ,createdby               ,createdon        ,modifiedby           ,modifiedon                ,attributes                ,isactive                  ,issuspended 
                )
                VALUES (
                   @name                      ,@bitmask                   ,@version                   ,@notes                     ,@createdby               ,@createdon        ,@modifiedby           ,@modifiedon                ,@attributes                ,@isactive                  ,@issuspended 
                )
                RETURNING id;
                ";
                roles.isactive = true;
                roles.version = 1;
                roles.createdon = DateTime.UtcNow;
                roles.createdby = requeststate.usercontext.id;
                roles.modifiedon = DateTime.UtcNow;
                roles.modifiedby = requeststate.usercontext.id;

                DbCommand command = db.GetCommand(query);

                db.AddParameter(command, "name                      ", DbTypes.Types.String).Value = String.IsNullOrEmpty(roles.name                      ) ? "" : roles.name                      ;
db.AddParameter(command, "bitmask                   ", DbTypes.Types.Integer).Value = roles.bitmask                   ;
db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = roles.version                   ;
db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(roles.notes                     ) ? "" : roles.notes                     ;
db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = roles.createdby               ;
db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = roles.createdon        ;
db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = roles.modifiedby           ;
db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = roles.modifiedon                ;
db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = roles.attributes                _json;
db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = roles.isactive                  ;
db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = roles.issuspended ;
                
                using (DbDataReader reader = await db.Execute(command))
                {
                    if (await reader.ReadAsync())
                    {
                        roles.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                    }
                }
            }
        public async Task<Roles> Update(Roles roles)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.UpdateTransaction(db, roles);
                }
            return roles;
        }
        public async Task<bool> UpdateTransaction(IDb db, Roles roles)
        {
            bool result = false;
                String query = @"
                UPDATE Roles
                    SET 
                        name                       = @name                      ,bitmask                    = @bitmask                   ,version                    = @version                   ,notes                      = @notes                     ,createdby                = @createdby               ,createdon         = @createdon        ,modifiedby            = @modifiedby           ,modifiedon                 = @modifiedon                ,attributes                 = @attributes                ,isactive                   = @isactive                  ,issuspended  = @issuspended ,
                        version = version + 1
                ";
                
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);

                queryBuilder.AddParameter("id", "=", "id", roles.id, DbTypes.Types.Long);

                if (roles.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", roles.version, DbTypes.Types.Integer);
                }

                var command = queryBuilder.GetCommand(db);
                
                roles.modifiedon = DateTime.UtcNow;
                roles.modifiedby = requeststate.usercontext.id;
                
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = roles.id;
db.AddParameter(command, "name                      ", DbTypes.Types.String).Value = String.IsNullOrEmpty(roles.name                      ) ? "" : roles.name                      ;
db.AddParameter(command, "bitmask                   ", DbTypes.Types.Integer).Value = roles.bitmask                   ;
db.AddParameter(command, "version                   ", DbTypes.Types.Integer).Value = roles.version                   ;
db.AddParameter(command, "notes                     ", DbTypes.Types.String).Value = String.IsNullOrEmpty(roles.notes                     ) ? "" : roles.notes                     ;
db.AddParameter(command, "createdby               ", DbTypes.Types.Long).Value = roles.createdby               ;
db.AddParameter(command, "createdon        ", DbTypes.Types.DateTime).Value = roles.createdon        ;
db.AddParameter(command, "modifiedby           ", DbTypes.Types.Long).Value = roles.modifiedby           ;
db.AddParameter(command, "modifiedon                ", DbTypes.Types.DateTime).Value = roles.modifiedon                ;
db.AddParameter(command, "attributes                ", DbTypes.Types.Json).Value = roles.attributes                _json;
db.AddParameter(command, "isactive                  ", DbTypes.Types.Boolean).Value = roles.isactive                  ;
db.AddParameter(command, "issuspended ", DbTypes.Types.Boolean).Value = roles.issuspended ;

                if (await db.ExecuteNonQuery(command) > 0)
                {
                    roles.version = roles.version + 1;
                    result = true;
                }
            return result;
        }
        public async Task<bool> Delete(RolesDeleteReq roles)
        {
             bool result = false;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.DeleteTransaction(db, roles);
                    
                }
            return result;
        }
        public async Task<bool> DeleteTransaction(IDb db, RolesDeleteReq roles)
        {
            bool result = false;
                String query = @"
                UPDATE Roles
                SET isactive = '0',
                    version = version + 1,
                    modifiedon = @modifiedon,
                    modifiedby = @modifiedby 
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                queryBuilder.AddParameter("id", "=", "id", roles.id, DbTypes.Types.Long);
                if (roles.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", roles.version, DbTypes.Types.Integer);
                }
                DbCommand command = queryBuilder.GetCommand(db);
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = roles.id;
                db.AddParameter(command, "version", DbTypes.Types.Integer).Value = roles.version;
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
