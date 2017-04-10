using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisWebSite
{
    public static class RedisHelper
    {
        private static ConnectionMultiplexer _connection;
        private static readonly object obj = new object();

        public static ConnectionMultiplexer GetRedisConn
        {
            get
            {
                if (_connection == null || !_connection.IsConnected)
                {
                    lock (obj)
                    {
                        var connStr = $"{ConfigHelper.RedisIP}:{ConfigHelper.RedisPort}";
                        _connection = ConnectionMultiplexer.Connect(connStr);
                    }
                }
                return _connection;
            }
        }





    }
}
