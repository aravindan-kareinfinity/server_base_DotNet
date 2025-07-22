using System.Text.Json.Serialization;
using System.Text.Json;
namespace PlanItNoww.Models
{
    public class Payments
    {
        public long id { get; set; }
public long userid { get; set; }
public long subscriptionid { get; set; }
public string paymentgateway { get; set; }
public string paymentid { get; set; }
public string orderid { get; set; }
public Decimal amount { get; set; }
public string currency { get; set; }
public string status { get; set; }
public DateTime paidat { get; set; }
public string failurereason { get; set; }
public string refundstatus { get; set; }
public Decimal refundedamount { get; set; }
public string receipturl { get; set; }

                                    public MetadataData metadata { get; set; }
                                    [JsonIgnore]
                                    public string metadata_json
                                    {
                                        get { return JsonSerializer.Serialize(metadata); }
                                        set
                                        {
                                            if (!string.IsNullOrEmpty(value) && value != "null")
                                                metadata = JsonSerializer.Deserialize<MetadataData>(value);
                                        }
                                    }
                                
public DateTime createdon { get; set; }
        
                public class MetadataData
                {

                }  
                
    }
    public class PaymentsSelectReq
    {
        public long id { get; set; }
    }
    public class PaymentsDeleteReq
    {
        public long id { get; set; }
        public int version { get; set; }
    }
}