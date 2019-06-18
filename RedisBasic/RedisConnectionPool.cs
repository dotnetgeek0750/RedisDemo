using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBasic
{
    public class RedisConnectionPool
    {
        private static ConcurrentQueue<ConnectionMultiplexer> connectionPoolQueue;
        private static int minConnectionNum;
        private static int maxConnectionNum;
        private static string host;
        private static int port;

        /// <summary>
        /// 通过构造函数 或者 config形式 获取 max，min host，port
        /// </summary>
        public static void InitializeConnectionPool()
        {
            minConnectionNum = 10;
            maxConnectionNum = 100;
            host = "192.168.9.128";
            port = 6379;
            connectionPoolQueue = new ConcurrentQueue<ConnectionMultiplexer>();

            for (int i = 0; i < minConnectionNum; i++)
            {
                var client = OpenConnection(host, port);
                PushConnection(client);
            }
            Console.WriteLine($"{0} 个 connection 初始化完毕！");
        }

        /*
        * 1. 如果说池中没有connection了，那么你需要OpenConnection
        * 
        * 2. 如果池中获取到了connection，并且isConnected=false，那么直接close
        * 
        */
        public static ConnectionMultiplexer GetConnection()
        {
            while (connectionPoolQueue.Count > 0)
            {
                connectionPoolQueue.TryDequeue(out ConnectionMultiplexer client);
                if (!client.IsConnected)
                {
                    client.Close();
                    continue;
                }
                return client;
            }
            return OpenConnection(host, port);
        }


        /// <summary>
        /// 1.  如果 queue的个数 >=max 直接踢掉
        /// 
        /// 2. client的IsConnected 如果为false， close 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static void PushConnection(ConnectionMultiplexer client)
        {
            if (connectionPoolQueue.Count >= maxConnectionNum)
            {
                client.Close();
                return;
            }

            if (!client.IsConnected)
            {
                client.Close();
                return;
            }

            connectionPoolQueue.Enqueue(client);
        }

        public static ConnectionMultiplexer OpenConnection(string host, int port)
        {
            ConnectionMultiplexer client = ConnectionMultiplexer.Connect($"{host}:{port}");
            return client;
        }
    }
}
