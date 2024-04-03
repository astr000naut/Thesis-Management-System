namespace TenantManagement.API.Param
{
    public class CheckDBConnectionParam
    {
        public bool AutoCreateDB { get; set; }
        public string ConnectionString { get; set; }
        public string DBName { get; set; }

    }
}
