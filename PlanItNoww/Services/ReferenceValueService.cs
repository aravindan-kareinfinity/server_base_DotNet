using PlanItNoww.Models;
using PlanItNoww.Utils;
using System.Data.Common;

namespace PlanItNoww.Services
{
    public class ReferenceValueService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;
        
        public ReferenceValueService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
        }
        public async Task<List<ReferenceValue>> Select(ReferenceValueSelectReq req)
        {
            List<ReferenceValue> result = null;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.SelectTransaction(db, req);
                }
            return result;
        }
        public async Task<List<ReferenceValue>> SelectTransaction(IDb db, ReferenceValueSelectReq req)
        {
            List<ReferenceValue> result = new List<ReferenceValue>();
                string query = @"
                SELECT ReferenceValue.id,ReferenceValue.identifier,ReferenceValue.displaytext,ReferenceValue.langcode,ReferenceValue.organizationid,ReferenceValue.version,ReferenceValue.createdby,ReferenceValue.createdon,ReferenceValue.modifiedby,ReferenceValue.modifiedon,ReferenceValue.attributes,ReferenceValue.isactive,ReferenceValue.issuspended,ReferenceValue.parentid,ReferenceValue.isfactory,ReferenceValue.notes
                FROM ReferenceValue
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                if (req.id > 0)
                {
                    queryBuilder.AddParameter("ReferenceValue.id", "=", "id", req.id, DbTypes.Types.Long);
                }
                if(req.parentid > 0)
            {
                queryBuilder.AddParameter("ReferenceValue.parentid", "=", "parentid", req.parentid, DbTypes.Types.Long);
            }

            if (req.referencetypeid > 0)
            {
                queryBuilder.AddParameter("ReferenceValue.referencetypeid", "=", "referencetypeid", req.referencetypeid, DbTypes.Types.Long);
            }
            queryBuilder.AddParameter("ReferenceValue.isactive", "=", "isactive", true, DbTypes.Types.Boolean);

                queryBuilder.AddOrderBy(QueryBuilder.Order.ASC, "ReferenceValue.identifier");
                var command = queryBuilder.GetCommand(db);
                using (DbDataReader reader = await db.Execute(command))
                {
                    while (await reader.ReadAsync())
                    {
                        ReferenceValue temp = new ReferenceValue();
                         temp.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
temp.identifier = reader["identifier"] == DBNull.Value ? "" : reader["identifier"].ToString();
temp.displaytext = reader["displaytext"] == DBNull.Value ? "" : reader["displaytext"].ToString();
temp.langcode = reader["langcode"] == DBNull.Value ? "" : reader["langcode"].ToString();
 temp.organizationid = reader["organizationid"] == DBNull.Value ? 0 : Convert.ToInt32(reader["organizationid"]);
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
        public async Task<ReferenceValue> Insert(ReferenceValue referencevalue)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.InsertTransaction(db, referencevalue);
                }
            return referencevalue;
        }
        public async Task InsertTransaction(IDb db, ReferenceValue referencevalue)
        {
                String query = @"
                INSERT INTO ReferenceValue (
                    identifier,displaytext,langcode,organizationid,version,createdby,createdon,modifiedby,modifiedon,attributes,isactive,issuspended,parentid,isfactory,notes
                )
                VALUES (
                   @identifier,@displaytext,@langcode,@organizationid,@version,@createdby,@createdon,@modifiedby,@modifiedon,@attributes,@isactive,@issuspended,@parentid,@isfactory,@notes
                )
                RETURNING id;
                ";
                referencevalue.isactive = true;
                referencevalue.version = 1;
                referencevalue.createdon = DateTime.UtcNow;
                referencevalue.createdby = requeststate.usercontext.id;
                referencevalue.modifiedon = DateTime.UtcNow;
                referencevalue.modifiedby = requeststate.usercontext.id;

                DbCommand command = db.GetCommand(query);

                db.AddParameter(command, "identifier", DbTypes.Types.String).Value = String.IsNullOrEmpty(referencevalue.identifier) ? "" : referencevalue.identifier;
db.AddParameter(command, "displaytext", DbTypes.Types.String).Value = String.IsNullOrEmpty(referencevalue.displaytext) ? "" : referencevalue.displaytext;
db.AddParameter(command, "langcode", DbTypes.Types.String).Value = String.IsNullOrEmpty(referencevalue.langcode) ? "" : referencevalue.langcode;
db.AddParameter(command, "organizationid", DbTypes.Types.Integer).Value = referencevalue.organizationid;
db.AddParameter(command, "version", DbTypes.Types.Integer).Value = referencevalue.version;
db.AddParameter(command, "createdby", DbTypes.Types.Long).Value = referencevalue.createdby;
db.AddParameter(command, "createdon", DbTypes.Types.DateTime).Value = referencevalue.createdon;
db.AddParameter(command, "modifiedby", DbTypes.Types.Long).Value = referencevalue.modifiedby;
db.AddParameter(command, "modifiedon", DbTypes.Types.DateTime).Value = referencevalue.modifiedon;
db.AddParameter(command, "attributes", DbTypes.Types.Json).Value = referencevalue.attributes_json;
db.AddParameter(command, "isactive", DbTypes.Types.Boolean).Value = referencevalue.isactive;
db.AddParameter(command, "issuspended", DbTypes.Types.Boolean).Value = referencevalue.issuspended;
db.AddParameter(command, "parentid", DbTypes.Types.Long).Value = referencevalue.parentid;
db.AddParameter(command, "isfactory", DbTypes.Types.Boolean).Value = referencevalue.isfactory;
db.AddParameter(command, "notes", DbTypes.Types.String).Value = String.IsNullOrEmpty(referencevalue.notes) ? "" : referencevalue.notes;
                
                using (DbDataReader reader = await db.Execute(command))
                {
                    if (await reader.ReadAsync())
                    {
                        referencevalue.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                    }
                }
            }
        public async Task<ReferenceValue> Update(ReferenceValue referencevalue)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.UpdateTransaction(db, referencevalue);
                }
            return referencevalue;
        }
        public async Task<bool> UpdateTransaction(IDb db, ReferenceValue referencevalue)
        {
            bool result = false;
                String query = @"
                UPDATE ReferenceValue
                    SET 
                        identifier = @identifier,displaytext = @displaytext,langcode = @langcode,organizationid = @organizationid,modifiedby = @modifiedby,modifiedon = @modifiedon,attributes = @attributes,issuspended = @issuspended,parentid = @parentid,isfactory = @isfactory,notes = @notes,
                        version = version + 1
                ";
                
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);

                queryBuilder.AddParameter("id", "=", "id", referencevalue.id, DbTypes.Types.Long);

                if (referencevalue.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", referencevalue.version, DbTypes.Types.Integer);
                }

                var command = queryBuilder.GetCommand(db);
                
                referencevalue.modifiedon = DateTime.UtcNow;
                referencevalue.modifiedby = requeststate.usercontext.id;
                
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = referencevalue.id;
db.AddParameter(command, "identifier", DbTypes.Types.String).Value = String.IsNullOrEmpty(referencevalue.identifier) ? "" : referencevalue.identifier;
db.AddParameter(command, "displaytext", DbTypes.Types.String).Value = String.IsNullOrEmpty(referencevalue.displaytext) ? "" : referencevalue.displaytext;
db.AddParameter(command, "langcode", DbTypes.Types.String).Value = String.IsNullOrEmpty(referencevalue.langcode) ? "" : referencevalue.langcode;
db.AddParameter(command, "organizationid", DbTypes.Types.Integer).Value = referencevalue.organizationid;
db.AddParameter(command, "version", DbTypes.Types.Integer).Value = referencevalue.version;
db.AddParameter(command, "modifiedby", DbTypes.Types.Long).Value = referencevalue.modifiedby;
db.AddParameter(command, "modifiedon", DbTypes.Types.DateTime).Value = referencevalue.modifiedon;
db.AddParameter(command, "attributes", DbTypes.Types.Json).Value = referencevalue.attributes_json;
db.AddParameter(command, "issuspended", DbTypes.Types.Boolean).Value = referencevalue.issuspended;
db.AddParameter(command, "parentid", DbTypes.Types.Long).Value = referencevalue.parentid;
db.AddParameter(command, "isfactory", DbTypes.Types.Boolean).Value = referencevalue.isfactory;
db.AddParameter(command, "notes", DbTypes.Types.String).Value = String.IsNullOrEmpty(referencevalue.notes) ? "" : referencevalue.notes;

                if (await db.ExecuteNonQuery(command) > 0)
                {
                    referencevalue.version = referencevalue.version + 1;
                    result = true;
                }
            return result;
        }
        public async Task<bool> Delete(ReferenceValueDeleteReq referencevalue)
        {
             bool result = false;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.DeleteTransaction(db, referencevalue);
                    
                }
            return result;
        }
        public async Task<bool> DeleteTransaction(IDb db, ReferenceValueDeleteReq referencevalue)
        {
            bool result = false;
                String query = @"
                UPDATE ReferenceValue
                SET isactive = '0',
                    version = version + 1,
                    modifiedon = @modifiedon,
                    modifiedby = @modifiedby 
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                queryBuilder.AddParameter("id", "=", "id", referencevalue.id, DbTypes.Types.Long);
                if (referencevalue.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", referencevalue.version, DbTypes.Types.Integer);
                }
                DbCommand command = queryBuilder.GetCommand(db);
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = referencevalue.id;
                db.AddParameter(command, "version", DbTypes.Types.Integer).Value = referencevalue.version;
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
