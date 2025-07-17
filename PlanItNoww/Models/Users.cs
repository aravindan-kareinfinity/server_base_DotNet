using System.Text.Json.Serialization;
using System.Text.Json;

namespace PlanItNoww.Models
{
    public class Users
    {
        public long id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string mobilecountrycode { get; set; }
        public string designation { get; set; }
        public string otp { get; set; }
        public DateTime otpexpirationtime { get; set; }
        public int version { get; set; }
        public long createdby { get; set; }
        public DateTime createdon { get; set; }
        public long modifiedby { get; set; }
        public DateTime modifiedon { get; set; }

        public AttributesData attributes { get; set; } = new AttributesData();
        [JsonIgnore]
        public string attributes_json
        {
            get { return JsonSerializer.Serialize(attributes); }
            set
            {
                if (!string.IsNullOrEmpty(value) && value != "null")
                    attributes = JsonSerializer.Deserialize<AttributesData>(value);
            }
        }

        public bool isactive { get; set; }
        public bool issuspended { get; set; }
        public long parentid { get; set; }
        public bool isfactory { get; set; }
        public string notes { get; set; }

        public class AttributesData
        {
            public UsersPermissionData permission { get; set; }
        }
    }

    public class UsersPermissionData
    {
        public UsersPermissionGroupData createstaff { get; set; } = new UsersPermissionGroupData();
        public UsersPermissionGroupData creategroup { get; set; } = new UsersPermissionGroupData();
        public UsersPermissionGroupData approveusersingroup { get; set; } = new UsersPermissionGroupData();
        public UsersPermissionGroupData createmessage { get; set; } = new UsersPermissionGroupData();
        public UsersPermissionGroupData createtask { get; set; } = new UsersPermissionGroupData();
        public UsersPermissionGroupData dashboard { get; set; } = new UsersPermissionGroupData();
        public UsersPermissionGroupData approveappoinment { get; set; } = new UsersPermissionGroupData();
    }

    public class UsersPermissionGroupData
    {
        public bool view { get; set; }
        public bool manage { get; set; }
    }

    public class UsersSelectReq
    {
        public long id { get; set; }
        public string mobile { get; set; }
    }

    public class UsersDeleteReq
    {
        public long id { get; set; }
        public int version { get; set; }
    }

    public class UsersGetOtpReq
    {
        public string mobile { get; set; }
    }

    public class UsersGetOtpRes
    {
        public string mobile { get; set; }
        public string name { get; set; }
    }

    public class UsersLoginReq
    {
        public string mobile { get; set; }
        public string otp { get; set; }
    }

    public class UsersRefereshTokenReq
    {
        public long userid { get; set; }
        public string refreshtoken { get; set; }
    }

    public class UsersContext
    {
        public long id { get; set; }
        public long userid { get; set; }
        public string usermobile { get; set; }
        public string username { get; set; }
        public string useremail { get; set; }
        public UsersPermissionData userpermission { get; set; }
        public string refreshtoken { get; set; }
        public string accesstoken { get; set; }
    }

    public class UserGenerateJwtTokenReq
    {
        public long userid { get; set; }
        public string usermobile { get; set; }
        public string username { get; set; }
        public string useremail { get; set; }
        public string permissionhex { get; set; }
    }
}