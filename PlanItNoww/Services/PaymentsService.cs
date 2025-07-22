using PlanItNoww.Models;
using PlanItNoww.Utils;
using System.Data.Common;

namespace PlanItNoww.Services
{
    public class PaymentsService
    {
        IDbProvider dbprovider;
        IQueryBuilderProvider querybuilderprovider;
        RequestState requeststate;
        
        public PaymentsService(IDbProvider dbprovider, IQueryBuilderProvider querybuilderprovider, RequestState requeststate)
        {
            this.dbprovider = dbprovider;
            this.querybuilderprovider = querybuilderprovider;
            this.requeststate = requeststate;
        }
        public async Task<List<Payments>> Select(PaymentsSelectReq req)
        {
            List<Payments> result = null;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.SelectTransaction(db, req);
                }
            return result;
        }
        public async Task<List<Payments>> SelectTransaction(IDb db, PaymentsSelectReq req)
        {
            List<Payments> result = new List<Payments>();
                string query = @"
                SELECT Payments.id,Payments.userid,Payments.subscriptionid,Payments.paymentgateway,Payments.paymentid,Payments.orderid,Payments.amount,Payments.currency,Payments.status,Payments.paidat,Payments.failurereason,Payments.refundstatus,Payments.refundedamount,Payments.receipturl,Payments.metadata,Payments.createdon
                FROM Payments
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                if (req.id > 0)
                {
                    queryBuilder.AddParameter("Payments.id", "=", "id", req.id, DbTypes.Types.Long);
                }
                queryBuilder.AddParameter("Payments.isactive", "=", "isactive", true, DbTypes.Types.Boolean);

                queryBuilder.AddOrderBy(QueryBuilder.Order.ASC, "Payments.id");
                var command = queryBuilder.GetCommand(db);
                using (DbDataReader reader = await db.Execute(command))
                {
                    while (await reader.ReadAsync())
                    {
                        Payments temp = new Payments();
                         temp.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
 temp.userid = reader["userid"] == DBNull.Value ? 0 : Convert.ToInt64(reader["userid"]);
 temp.subscriptionid = reader["subscriptionid"] == DBNull.Value ? 0 : Convert.ToInt64(reader["subscriptionid"]);
temp.paymentgateway = reader["paymentgateway"] == DBNull.Value ? "" : reader["paymentgateway"].ToString();
temp.paymentid = reader["paymentid"] == DBNull.Value ? "" : reader["paymentid"].ToString();
temp.orderid = reader["orderid"] == DBNull.Value ? "" : reader["orderid"].ToString();
 temp.amount = reader["amount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["amount"]);
temp.currency = reader["currency"] == DBNull.Value ? "" : reader["currency"].ToString();
temp.status = reader["status"] == DBNull.Value ? "" : reader["status"].ToString();
temp.paidat = reader["paidat"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["paidat"]);
temp.failurereason = reader["failurereason"] == DBNull.Value ? "" : reader["failurereason"].ToString();
temp.refundstatus = reader["refundstatus"] == DBNull.Value ? "" : reader["refundstatus"].ToString();
 temp.refundedamount = reader["refundedamount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["refundedamount"]);
temp.receipturl = reader["receipturl"] == DBNull.Value ? "" : reader["receipturl"].ToString();
temp.metadata_json = reader["metadata"] == DBNull.Value ? "null" : reader["metadata"].ToString();
temp.createdon = reader["createdon"] == DBNull.Value ? Base.GetMinimumDate() : Convert.ToDateTime(reader["createdon"]);
                        result.Add(temp);
                    }
                }
            return result;
        }
        public async Task<Payments> Insert(Payments payments)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.InsertTransaction(db, payments);
                }
            return payments;
        }
        public async Task InsertTransaction(IDb db, Payments payments)
        {
                String query = @"
                INSERT INTO Payments (
                    userid,subscriptionid,paymentgateway,paymentid,orderid,amount,currency,status,paidat,failurereason,refundstatus,refundedamount,receipturl,metadata,createdon
                )
                VALUES (
                   @userid,@subscriptionid,@paymentgateway,@paymentid,@orderid,@amount,@currency,@status,@paidat,@failurereason,@refundstatus,@refundedamount,@receipturl,@metadata,@createdon
                )
                RETURNING id;
                ";
                payments.isactive = true;
                payments.version = 1;
                payments.createdon = DateTime.UtcNow;
                payments.createdby = requeststate.usercontext.id;
                payments.modifiedon = DateTime.UtcNow;
                payments.modifiedby = requeststate.usercontext.id;

                DbCommand command = db.GetCommand(query);

                db.AddParameter(command, "userid", DbTypes.Types.Long).Value = payments.userid;
db.AddParameter(command, "subscriptionid", DbTypes.Types.Long).Value = payments.subscriptionid;
db.AddParameter(command, "paymentgateway", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.paymentgateway) ? "" : payments.paymentgateway;
db.AddParameter(command, "paymentid", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.paymentid) ? "" : payments.paymentid;
db.AddParameter(command, "orderid", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.orderid) ? "" : payments.orderid;
db.AddParameter(command, "amount", DbTypes.Types.Decimal).Value = payments.amount;
db.AddParameter(command, "currency", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.currency) ? "" : payments.currency;
db.AddParameter(command, "status", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.status) ? "" : payments.status;
db.AddParameter(command, "paidat", DbTypes.Types.DateTime).Value = payments.paidat;
db.AddParameter(command, "failurereason", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.failurereason) ? "" : payments.failurereason;
db.AddParameter(command, "refundstatus", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.refundstatus) ? "" : payments.refundstatus;
db.AddParameter(command, "refundedamount", DbTypes.Types.Decimal).Value = payments.refundedamount;
db.AddParameter(command, "receipturl", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.receipturl) ? "" : payments.receipturl;
db.AddParameter(command, "metadata", DbTypes.Types.Json).Value = payments.metadata_json;
db.AddParameter(command, "createdon", DbTypes.Types.DateTime).Value = payments.createdon;
                
                using (DbDataReader reader = await db.Execute(command))
                {
                    if (await reader.ReadAsync())
                    {
                        payments.id = reader["id"] == DBNull.Value ? 0 : Convert.ToInt64(reader["id"]);
                    }
                }
            }
        public async Task<Payments> Update(Payments payments)
        {
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    await this.UpdateTransaction(db, payments);
                }
            return payments;
        }
        public async Task<bool> UpdateTransaction(IDb db, Payments payments)
        {
            bool result = false;
                String query = @"
                UPDATE Payments
                    SET 
                        userid = @userid,subscriptionid = @subscriptionid,paymentgateway = @paymentgateway,paymentid = @paymentid,orderid = @orderid,amount = @amount,currency = @currency,status = @status,paidat = @paidat,failurereason = @failurereason,refundstatus = @refundstatus,refundedamount = @refundedamount,receipturl = @receipturl,metadata = @metadata,
                        version = version + 1
                ";
                
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);

                queryBuilder.AddParameter("id", "=", "id", payments.id, DbTypes.Types.Long);

                if (payments.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", payments.version, DbTypes.Types.Integer);
                }

                var command = queryBuilder.GetCommand(db);
                
                payments.modifiedon = DateTime.UtcNow;
                payments.modifiedby = requeststate.usercontext.id;
                
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = payments.id;
db.AddParameter(command, "userid", DbTypes.Types.Long).Value = payments.userid;
db.AddParameter(command, "subscriptionid", DbTypes.Types.Long).Value = payments.subscriptionid;
db.AddParameter(command, "paymentgateway", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.paymentgateway) ? "" : payments.paymentgateway;
db.AddParameter(command, "paymentid", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.paymentid) ? "" : payments.paymentid;
db.AddParameter(command, "orderid", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.orderid) ? "" : payments.orderid;
db.AddParameter(command, "amount", DbTypes.Types.Decimal).Value = payments.amount;
db.AddParameter(command, "currency", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.currency) ? "" : payments.currency;
db.AddParameter(command, "status", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.status) ? "" : payments.status;
db.AddParameter(command, "paidat", DbTypes.Types.DateTime).Value = payments.paidat;
db.AddParameter(command, "failurereason", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.failurereason) ? "" : payments.failurereason;
db.AddParameter(command, "refundstatus", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.refundstatus) ? "" : payments.refundstatus;
db.AddParameter(command, "refundedamount", DbTypes.Types.Decimal).Value = payments.refundedamount;
db.AddParameter(command, "receipturl", DbTypes.Types.String).Value = String.IsNullOrEmpty(payments.receipturl) ? "" : payments.receipturl;
db.AddParameter(command, "metadata", DbTypes.Types.Json).Value = payments.metadata_json;

                if (await db.ExecuteNonQuery(command) > 0)
                {
                    payments.version = payments.version + 1;
                    result = true;
                }
            return result;
        }
        public async Task<bool> Delete(PaymentsDeleteReq payments)
        {
             bool result = false;
                using (IDb db = await dbprovider.GetDb())
                {
                    await db.Connect();
                    result = await this.DeleteTransaction(db, payments);
                    
                }
            return result;
        }
        public async Task<bool> DeleteTransaction(IDb db, PaymentsDeleteReq payments)
        {
            bool result = false;
                String query = @"
                UPDATE Payments
                SET isactive = '0',
                    version = version + 1,
                    modifiedon = @modifiedon,
                    modifiedby = @modifiedby 
                ";
                var queryBuilder = querybuilderprovider.GetQueryBuilder(query);
                queryBuilder.AddParameter("id", "=", "id", payments.id, DbTypes.Types.Long);
                if (payments.version > 0)
                {
                    queryBuilder.AddParameter("version", "=", "version", payments.version, DbTypes.Types.Integer);
                }
                DbCommand command = queryBuilder.GetCommand(db);
                db.AddParameter(command, "id", DbTypes.Types.Long).Value = payments.id;
                db.AddParameter(command, "version", DbTypes.Types.Integer).Value = payments.version;
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
