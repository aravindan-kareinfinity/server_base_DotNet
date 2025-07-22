using PlanItNoww.Models;
using PlanItNoww.Utils;
using System.Data.Common;

namespace PlanItNoww.Services
{
    public class SubscriptionsService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;
        
        public SubscriptionsService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
        }
        public async Task<List<Subscriptions>> Select(SubscriptionsSelectReq req)
        {
            List<Subscriptions> result = null;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.SelectTransaction(db, req);
                }
            return result;
        }
        public async Task<List<Subscriptions>> SelectTransaction(IDb db, SubscriptionsSelectReq req)
        {
            List<Subscriptions> result = new List<Subscriptions>();
                string query = @"
                SELECT Subscriptions.id,Subscriptions.userid,Subscriptions.planname,Subscriptions.plantype,Subscriptions.amountpaid,Subscriptions.startdate,Subscriptions.enddate,Subscriptions.isactive,Subscriptions.issuspended,Subscriptions.isrenewable,Subscriptions.graceperioddays,Subscriptions.cancellationreason
                FROM Subscriptions
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                if (req.id > 0)
                {
                    queryBuilder.AddParameter("Subscriptions.id", "=", "id", req.id, DbTypes.Types.Long);
                }
                queryBuilder.AddParameter("Subscriptions.isactive", "=", "isactive", true, DbTypes.Types.Boolean);

                queryBuilder.AddOrderBy(QueryBuilder.Order.ASC, "Subscriptions.id");
                var command = queryBuilder.GetCommand(db);
                using (DbDataReader reader = await db.Execute(command))
                {
                    while (await reader.ReadAsync())
                    {
                        Subscriptions temp = new Subscriptions();
                         temp.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
temp.userid = reader["userid"] == DBNull.Value ? "" : reader["userid"].ToString();
temp.planname = reader["planname"] == DBNull.Value ? "" : reader["planname"].ToString();
temp.plantype = reader["plantype"] == DBNull.Value ? "" : reader["plantype"].ToString();
 temp.amountpaid = reader["amountpaid"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["amountpaid"]);
temp.startdate = reader["startdate"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["startdate"]);
temp.enddate = reader["enddate"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["enddate"]);
 temp.isactive = reader["isactive"] == DBNull.Value ? false : Convert.ToBoolean(reader["isactive"]);
 temp.issuspended = reader["issuspended"] == DBNull.Value ? false : Convert.ToBoolean(reader["issuspended"]);
 temp.isrenewable = reader["isrenewable"] == DBNull.Value ? false : Convert.ToBoolean(reader["isrenewable"]);
 temp.graceperioddays = reader["graceperioddays"] == DBNull.Value ? 0 : Convert.ToInt32(reader["graceperioddays"]);
temp.cancellationreason = reader["cancellationreason"] == DBNull.Value ? "" : reader["cancellationreason"].ToString();
                        result.Add(temp);
                    }
                }
            return result;
        }
        public async Task<Subscriptions> Insert(Subscriptions subscriptions)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.InsertTransaction(db, subscriptions);
                }
            return subscriptions;
        }
        public async Task InsertTransaction(IDb db, Subscriptions subscriptions)
        {
                String query = @"
                INSERT INTO Subscriptions (
                    userid,planname,plantype,amountpaid,startdate,enddate,isactive,issuspended,isrenewable,graceperioddays,cancellationreason
                )
                VALUES (
                   @userid,@planname,@plantype,@amountpaid,@startdate,@enddate,@isactive,@issuspended,@isrenewable,@graceperioddays,@cancellationreason
                )
                RETURNING id;
                ";
                subscriptions.isactive = true;
                subscriptions.version = 1;
                subscriptions.createdon = DateTime.UtcNow;
                subscriptions.createdby = requeststate.usercontext.id;
                subscriptions.modifiedon = DateTime.UtcNow;
                subscriptions.modifiedby = requeststate.usercontext.id;

                DbCommand command = db.GetCommand(query);

                db.AddParameter(command, "userid", DbTypes.Types.String).Value = String.IsNullOrEmpty(subscriptions.userid) ? "" : subscriptions.userid;
db.AddParameter(command, "planname", DbTypes.Types.String).Value = String.IsNullOrEmpty(subscriptions.planname) ? "" : subscriptions.planname;
db.AddParameter(command, "plantype", DbTypes.Types.String).Value = String.IsNullOrEmpty(subscriptions.plantype) ? "" : subscriptions.plantype;
db.AddParameter(command, "amountpaid", DbTypes.Types.Decimal).Value = subscriptions.amountpaid;
db.AddParameter(command, "startdate", DbTypes.Types.DateTime).Value = subscriptions.startdate;
db.AddParameter(command, "enddate", DbTypes.Types.DateTime).Value = subscriptions.enddate;
db.AddParameter(command, "isactive", DbTypes.Types.Boolean).Value = subscriptions.isactive;
db.AddParameter(command, "issuspended", DbTypes.Types.Boolean).Value = subscriptions.issuspended;
db.AddParameter(command, "isrenewable", DbTypes.Types.Boolean).Value = subscriptions.isrenewable;
db.AddParameter(command, "graceperioddays", DbTypes.Types.Integer).Value = subscriptions.graceperioddays;
db.AddParameter(command, "cancellationreason", DbTypes.Types.String).Value = String.IsNullOrEmpty(subscriptions.cancellationreason) ? "" : subscriptions.cancellationreason;
                
                using (DbDataReader reader = await db.Execute(command))
                {
                    if (await reader.ReadAsync())
                    {
                        subscriptions.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                    }
                }
            }
        public async Task<Subscriptions> Update(Subscriptions subscriptions)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.UpdateTransaction(db, subscriptions);
                }
            return subscriptions;
        }
        public async Task<bool> UpdateTransaction(IDb db, Subscriptions subscriptions)
        {
            bool result = false;
                String query = @"
                UPDATE Subscriptions
                    SET 
                        userid = @userid,planname = @planname,plantype = @plantype,amountpaid = @amountpaid,startdate = @startdate,enddate = @enddate,issuspended = @issuspended,isrenewable = @isrenewable,graceperioddays = @graceperioddays,cancellationreason = @cancellationreason,
                        version = version + 1
                ";
                
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);

                queryBuilder.AddParameter("id", "=", "id", subscriptions.id, DbTypes.Types.Long);

                if (subscriptions.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", subscriptions.version, DbTypes.Types.Integer);
                }

                var command = queryBuilder.GetCommand(db);
                
                subscriptions.modifiedon = DateTime.UtcNow;
                subscriptions.modifiedby = requeststate.usercontext.id;
                
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = subscriptions.id;
db.AddParameter(command, "userid", DbTypes.Types.String).Value = String.IsNullOrEmpty(subscriptions.userid) ? "" : subscriptions.userid;
db.AddParameter(command, "planname", DbTypes.Types.String).Value = String.IsNullOrEmpty(subscriptions.planname) ? "" : subscriptions.planname;
db.AddParameter(command, "plantype", DbTypes.Types.String).Value = String.IsNullOrEmpty(subscriptions.plantype) ? "" : subscriptions.plantype;
db.AddParameter(command, "amountpaid", DbTypes.Types.Decimal).Value = subscriptions.amountpaid;
db.AddParameter(command, "startdate", DbTypes.Types.DateTime).Value = subscriptions.startdate;
db.AddParameter(command, "enddate", DbTypes.Types.DateTime).Value = subscriptions.enddate;
db.AddParameter(command, "issuspended", DbTypes.Types.Boolean).Value = subscriptions.issuspended;
db.AddParameter(command, "isrenewable", DbTypes.Types.Boolean).Value = subscriptions.isrenewable;
db.AddParameter(command, "graceperioddays", DbTypes.Types.Integer).Value = subscriptions.graceperioddays;
db.AddParameter(command, "cancellationreason", DbTypes.Types.String).Value = String.IsNullOrEmpty(subscriptions.cancellationreason) ? "" : subscriptions.cancellationreason;

                if (await db.ExecuteNonQuery(command) > 0)
                {
                    subscriptions.version = subscriptions.version + 1;
                    result = true;
                }
            return result;
        }
        public async Task<bool> Delete(SubscriptionsDeleteReq subscriptions)
        {
             bool result = false;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.DeleteTransaction(db, subscriptions);
                    
                }
            return result;
        }
        public async Task<bool> DeleteTransaction(IDb db, SubscriptionsDeleteReq subscriptions)
        {
            bool result = false;
                String query = @"
                UPDATE Subscriptions
                SET isactive = '0',
                    version = version + 1,
                    modifiedon = @modifiedon,
                    modifiedby = @modifiedby 
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                queryBuilder.AddParameter("id", "=", "id", subscriptions.id, DbTypes.Types.Long);
                if (subscriptions.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", subscriptions.version, DbTypes.Types.Integer);
                }
                DbCommand command = queryBuilder.GetCommand(db);
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = subscriptions.id;
                db.AddParameter(command, "version", DbTypes.Types.Integer).Value = subscriptions.version;
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
