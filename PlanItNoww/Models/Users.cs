using System.Text.Json.Serialization;
using System.Text.Json;
namespace PlanItNoww.Models
{
    public class Users
    {
        public long id { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string passwordhash { get; set; }
        public string googleid { get; set; }
        public long roleid { get; set; }
        public string pushnotificationtoken { get; set; }
        public bool isemailverified { get; set; }
        public bool ismobileverified { get; set; }
        public bool isaadhaarverified { get; set; }
        public int version { get; set; }
        public string notes { get; set; }
        public long createdby { get; set; }
        public DateTime createdon { get; set; }
        public long modifiedby { get; set; }
        public DateTime modifiedon { get; set; }

        public bool isactive { get; set; }
        public bool issuspended { get; set; }

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




    

                
    }
    public class AttributesData
    {

                }
public class UsersSelectReq
    {
        public long id { get; set; }
    }
    public class UsersDeleteReq
    {
        public long id { get; set; }
        public int version { get; set; }
    }
    public class UsersContext
    {
        public long userid { get; set; }
        public string usermobile { get; set; }
        public string username { get; set; }
        public string useremail { get; set; }
        public string refreshtoken { get; set; }
        public string accesstoken { get; set; }
    }
}