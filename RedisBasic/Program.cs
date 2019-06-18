using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisBasic
{
    class Program
    {
        static ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("192.168.9.128:6379");
        static IDatabase db = redis.GetDatabase(0);

        //1、String的应用，可以用来实现session
        static void RedisString()
        {
            //1、String的应用，可以用来实现session
            db.StringSet("sessionId", "wayne", TimeSpan.FromSeconds(10));
            while (true)
            {
                var info = db.StringGet("sessionId");
                if (info.HasValue)
                {
                    Console.WriteLine(info);
                }
                else
                {
                    Console.WriteLine("session失效");
                }
                Thread.Sleep(1000);
            }
        }

        //2、Hash的应用：可以用来存放多个链接字符串
        static void RedisHash()
        {
            //2、Hash的应用：可以用来存放多个链接字符串
            db.HashSet("connections", "1", "mysql://192.168.1.1/mydb");
            db.HashSet("connections", "2", "mysql://192.168.1.2/mydb");
            db.HashSet("connections", "3", "mysql://192.168.1.3/mydb");
        }

        //3、Set的应用：可以用来存放黑名单
        static void RedisSet()
        {
            ////3、Set的应用：可以用来存放黑名单
            db.SetAdd("blacklist", "1");
            db.SetAdd("blacklist", "2");
            db.SetAdd("blacklist", "3");
            var b = db.SetContains("blacklist", "2");
        }

        //4、List的应用：可以实现消息队列
        static void RedisList()
        {
            //4、List的应用：可以实现消息队列
            db.ListLeftPush("smsqueue", "1311111111");
            db.ListLeftPush("smsqueue", "1322222222");
            db.ListLeftPush("smsqueue", "1333333333");
            db.ListLeftPush("smsqueue", "1344444444");

            Console.WriteLine(db.ListRightPop("smsqueue"));
        }


        //redis 连接池
        static void RedisConnPool()
        {
            RedisConnectionPool.InitializeConnectionPool();
            for (int m = 0; m < 1000000; m++)
            {
                ConnectionMultiplexer client = null;
                try
                {
                    client = RedisConnectionPool.GetConnection();
                    var db = client.GetDatabase(0);

                    db.StringSet("username", "jack");

                    Console.WriteLine(db.StringGet("username") + " " + m);
                }
                finally
                {
                    if (client != null)
                    {
                        RedisConnectionPool.PushConnection(client);
                    }
                }
                //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("192.168.181.131:6379");
                //Console.WriteLine(m);
            }
        }


        #region 秒杀StringIncrement

        //秒杀
        static void SpikeBuying()
        {
            while (true)
            {
                var num = db.StringIncrement("MaxProductNum");
                if (num > 3000)
                {
                    Console.WriteLine("库存不足");
                }
                else
                {
                    Console.WriteLine("可购买");
                }
            }
        }

        #endregion

        static void BitMap()
        {
            db.StringSetBit("blacklist", 1, true);
            db.StringSetBit("blacklist", 3, true);
            db.StringSetBit("blacklist", 5, true);

            for (long i = 0; i < long.MaxValue; i++)
            {
                db.StringSetBit("blacklist", i, true);
            }
            var j = 0;
        }


        static void Hash()
        {
            db.HashSet("conn", 1, "mysql://1");
            db.HashSet("conn", 2, "mysql://2");
            db.HashSet("conn", 3, "mysql://3");

            var val = db.HashGet("conn", 2);

            var len = db.HashLength("conn");
            var hasKye = db.HashKeys("conn");
        }


        static void Set()
        {
            db.SetAdd("blacklist", 1);

            var arr = db.SetMembers("blacklist");
            Console.WriteLine(string.Join(",", arr));

            var len = db.SetLength("blacklist");

            db.SetRemove("blacklist", 2);
        }


        static void SortSet()
        {
            var rand = new Random();
            for (int i = 1; i <= 10000; i++)
            {
                var customerID = i;
                var totalTradeMoney = rand.Next(10, 10000);
                db.SortedSetAdd("shop", i, totalTradeMoney);
            }
            Console.WriteLine("插入成功！");
        }

        static void SortSet25()
        {
            var len = db.SortedSetLength("shop");
            var customerRank = len * 0.25;
            // 高端客户
            var customerID = 60;
            var dbRank = db.SortedSetRank("shop", customerID, Order.Descending);
            Console.ReadKey();
        }

        #region 发布/订阅


        static void SubPub()
        {
            var subscriber = redis.GetSubscriber();
            //非模式订阅，普通订阅
            subscriber.Subscribe("trade", (channel, redisValue) =>
            {
                Console.WriteLine($"message:{redisValue}");
            });

            //模式订阅
            var redisChannel = new RedisChannel("trad[ae]", RedisChannel.PatternMode.Pattern);
            subscriber.Subscribe(redisChannel, (channel, redisValue) =>
            {
                Console.WriteLine($"message:{redisValue}");
            });


            //发布
            subscriber.Publish("trade", "t11111111111111");
        }

        #endregion

        #region 事务

        static void Transactions()
        {
            var transaction = db.CreateTransaction();
            transaction.StringSetAsync("username", "tom");
            transaction.StringSetAsync("password", "1234");
            transaction.Execute();
            Console.WriteLine("提交成功");
        }

        #endregion

        #region 正则匹配Key

        /// <summary>
        /// 
        /// </summary>
        static void TestData4SCAN()
        {
            var rand = new Random();
            for (int i = 1; i < int.MaxValue; i++)
            {
                var customerID = rand.Next(1, 10000);
                var key = $"s{i}c{customerID}";
                var value = "";  //统计信息
                db.StringSet(key, value);
            }
            Console.WriteLine("提交成功！");
        }

        static void SCAN()
        {
            var server = redis.GetServer("192.168.9.128:6379");
            var list = server.Keys(0, "s*", 10);
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
            Console.ReadKey();
        }

        #endregion


        #region 大批量插入


        static void BatchInsert()
        {
            var dict = new Dictionary<int, List<KeyValuePair<RedisKey, RedisValue>>>();

            //组装数据，总共100*10000个KEY
            //模拟100个店铺
            for (int i = 0; i <= 100; i++)
            {
                //每个下面有10000个KEY
                var key = Guid.NewGuid().ToString();
                var smallList = Enumerable.Range(0, 10000)
                    .Select(m => new KeyValuePair<RedisKey, RedisValue>(key, key))
                    .ToList();
                dict.Add(i, smallList);
            }

            var stopwatch = Stopwatch.StartNew();

            //1、transaction
            foreach (var item in dict)
            {
                var transaction = db.CreateTransaction();
                foreach (var model in item.Value)
                {
                    transaction.StringSetAsync(model.Key, model.Value);
                }
                transaction.Execute();
            }

            Console.WriteLine($"transaction耗费的时间：{stopwatch.ElapsedMilliseconds}");
            stopwatch.Restart();


            //2、mset
            foreach (var item in dict)
            {
                db.StringSet(item.Value.ToArray());
            }
            Console.WriteLine($"mset耗费的时间：{stopwatch.ElapsedMilliseconds}");
            stopwatch.Restart();

            //3、pipeline 
            foreach (var item in dict)
            {
                var batch = db.CreateBatch();
                foreach (var model in item.Value)
                {
                    batch.StringSetAsync(model.Key, model.Value);
                }
                batch.Execute();
            }
            Console.WriteLine($"pipeline batch耗费的时间：{stopwatch.ElapsedMilliseconds}");
            stopwatch.Restart();


            //4、普通 做法
            foreach (var item in dict)
            {
                foreach (var model in item.Value)
                {
                    db.StringSet(model.Key, model.Value);
                }
            }
            Console.WriteLine($"普通耗费的时间：{stopwatch.ElapsedMilliseconds}");
            stopwatch.Restart();


            //5、lua脚本
            foreach (var item in dict)
            {
                var list = item.Value.Select(p => new model
                {
                    k = p.Key,
                    v = p.Value,
                })
                .ToList();
                db.ScriptEvaluate(File.ReadAllText(@"C:\\lua\batch.txt", Encoding.Default), new RedisKey[] { JsonConvert.SerializeObject(list) });
                Console.WriteLine($"lua {item.Key} 批次执行完毕");
            }
            Console.WriteLine($"lua脚本耗费的时间：{stopwatch.ElapsedMilliseconds}");
            stopwatch.Restart();
        }

        #endregion

        static void Main(string[] args)
        {
            SCAN();
        }
    }

    public class model
    {
        public string k { get; set; }
        public string v { get; set; }
    }
}
