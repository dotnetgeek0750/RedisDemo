using System.Configuration;

namespace RedisDemoConsole
{
    public static class ConfigHelper
    {
        public static string RedisIP
        {
            get
            {
                return ConfigurationManager.AppSettings["redisip"].ToString();
            }
        }


        public static string RedisPort
        {
            get
            {
                return ConfigurationManager.AppSettings["redisport"].ToString();
            }
        }
    }
}
