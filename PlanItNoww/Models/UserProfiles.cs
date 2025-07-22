using System.Text.Json.Serialization;
using System.Text.Json;
namespace PlanItNoww.Models
{
    public class UserProfiles
    {
        public long id { get; set; }
        public string userid { get; set; }
        public string fullname { get; set; }
        public DateTime dob { get; set; }
        public string gender { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }
        public string country { get; set; }
        public string profileimageurl { get; set; }
        public int version { get; set; }
        public string notes { get; set; }
        public long createdby { get; set; }
        public DateTime createdon { get; set; }
        public long modifiedby { get; set; }
        public DateTime modifiedon { get; set; }

        public AttributesData Attributes { get; set; }

        [JsonIgnore]
        public string attributes_json
        {
            get { return JsonSerializer.Serialize(Attributes); }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != "null")
                    Attributes = JsonSerializer.Deserialize<AttributesData>(value);
            }
        }

        public bool isactive { get; set; }
public bool issuspended { get; set; }

public class AttributesData
                {

                }  
                
    }
    public class UserProfilesSelectReq
{
    public long id { get; set; }
}
public class UserProfilesDeleteReq
{
    public long id { get; set; }
    public int version { get; set; }
}
}