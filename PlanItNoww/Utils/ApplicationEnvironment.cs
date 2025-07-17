
namespace PlanItNoww.Utils
{
    public class ApplicationEnvironment
    {
        public string postgresqlconnection { get; set; }
        public ApplicationEnvironmentAwsS3ConfigData awss3config { get; set; } = new ApplicationEnvironmentAwsS3ConfigData();
        public string jwtsecret { get; set; }
        
    }
    public class ApplicationEnvironmentAwsS3ConfigData
    {
        public bool iss3enabled { get; set; }
        public string endpoint { get; set; }
        public string accesskey { get; set; }
        public string secretaccesskey { get; set; }
        public string bucketname { get; set; }
        public string path { get; set; }
    }
}
