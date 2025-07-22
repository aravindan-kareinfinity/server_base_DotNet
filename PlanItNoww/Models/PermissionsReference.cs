using System.Text.Json.Serialization;
using System.Text.Json;
namespace PlanItNoww.Models
{
    public class PermissionsReference
    {
        public long id { get; set; }
public int bitposition                { get; set; }
public int value                      { get; set; }
public string description                { get; set; }
public string name { get; set; }
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
    public class PermissionsReferenceSelectReq
    {
        public long id { get; set; }
    }
    public class PermissionsReferenceDeleteReq
    {
        public long id { get; set; }
        public int version { get; set; }
    }
}