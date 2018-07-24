namespace PetProduct.Configuration
{
    public class ConnectionSettings
    {
        public string PostgreSQLConnectionString { get; set; }
        public string RedisConnectionString { get; set; }
        public string CachingEnabled { get; set; }
    }
}
