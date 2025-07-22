using System.Text.Json.Serialization;
using System.Text.Json;
namespace PlanItNoww.Models
{
    public class OTPs
    {
        public long id { get; set; }
public string userid { get; set; }
public string emailormobile              { get; set; }
public string otpcode                    { get; set; }
public string purpose                    { get; set; }
public DateTime expiresat                  { get; set; }
public bool isused                    { get; set; }
public int version                    { get; set; }
public string notes                      { get; set; }
public long createdby                { get; set; }
public DateTime createdon         { get; set; }
public long modifiedby            { get; set; }
public DateTime modifiedon                 { get; set; }

                                    public Attributes                Data attributes                 { get; set; }
                                    [JsonIgnore]
                                    public string attributes                _json
                                    {
                                        get { return JsonSerializer.Serialize(attributes                ); }
                                        set
                                        {
                                            if (!string.IsNullOrEmpty(value) && value != "null")
                                                attributes                 = JsonSerializer.Deserialize<Attributes                Data>(value);
                                        }
                                    }
                                
public bool isactive                   { get; set; }
public bool issuspended  { get; set; }
        
                public class Attributes                Data
                {

                }  
                
    }
    public class OTPsSelectReq
    {
        public long id { get; set; }
    }
    public class OTPsDeleteReq
    {
        public long id { get; set; }
        public int version { get; set; }
    }
}