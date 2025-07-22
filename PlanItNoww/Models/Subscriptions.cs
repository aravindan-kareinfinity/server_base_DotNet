using System.Text.Json.Serialization;
using System.Text.Json;
namespace PlanItNoww.Models
{
    public class Subscriptions
    {
        public long id { get; set; }
public string userid { get; set; }
public string planname { get; set; }
public string plantype { get; set; }
public Decimal amountpaid { get; set; }
public DateTime startdate { get; set; }
public DateTime enddate { get; set; }
public bool isactive { get; set; }
public bool issuspended { get; set; }
public bool isrenewable { get; set; }
public int graceperioddays { get; set; }
public string cancellationreason { get; set; }
        
    }
    public class SubscriptionsSelectReq
    {
        public long id { get; set; }
    }
    public class SubscriptionsDeleteReq
    {
        public long id { get; set; }
        public int version { get; set; }
    }
}