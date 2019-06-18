using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ListConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                RedisValue res;
                using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379"))
                {
                    IDatabase db = redis.GetDatabase();//默认是访问db0数据库，可以通过方法参数指定数字访问不同的数据库
                    res = db.ListRightPop("RedisListMail");
                }
                if (res.HasValue)
                {
                    Console.WriteLine("给用户发邮件：" + res);
                }
                else
                {
                    Console.WriteLine("没有待发送的邮件任务，休眠2秒钟");
                    Thread.Sleep(2000);
                }
            }
        }
    }
}
