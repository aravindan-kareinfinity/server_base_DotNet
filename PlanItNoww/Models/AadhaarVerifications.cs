using System.Text.Json.Serialization;
using System.Text.Json;
namespace PlanItNoww.Models
{
    public class AadhaarVerifications
    {
        public long id { get; set; }
public long userid { get; set; }
public string aadhaarnumbermasked { get; set; }
public string verificationstatus { get; set; }
public string verificationsource { get; set; }
public DateTime verifiedat { get; set; }

                                    public ResponsedataData responsedata { get; set; }
                                    [JsonIgnore]
                                    public string responsedata_json
                                    {
                                        get { return JsonSerializer.Serialize(responsedata); }
                                        set
                                        {
                                            if (!string.IsNullOrEmpty(value) && value != "null")
                                                responsedata = JsonSerializer.Deserialize<ResponsedataData>(value);
                                        }
                                    }
                                
public long createdby { get; set; }
public DateTime createdon { get; set; }
public long modifiedby { get; set; }
public DateTime modifiedon { get; set; }
public int version { get; set; }
public bool isactive { get; set; }
public bool issuspended { get; set; }
        
                public class ResponsedataData
                {

                }  
                
    }
    public class AadhaarVerificationsSelectReq
    {
        public long id { get; set; }
    }
    public class AadhaarVerificationsDeleteReq
    {
        public long id { get; set; }
        public int version { get; set; }
    }
}