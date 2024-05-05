namespace TenantManagement.API.Param
{
    public class CheckConnectionParam
    {
        public bool AutoCreateDB { get; set; }
        public string ConnectionString { get; set; }
        public string DBName { get; set; }

        public bool AutoCreateMinio { get; set; }
        public string MinioEndpoint { get; set; }
        public string MinioAccessKey { get; set; }
        public string MinioSecretKey { get; set; }
        public string MinioBucketName { get; set; }

    }
}
