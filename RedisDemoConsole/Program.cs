using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RedisDemoConsole.Entity;
using CtripSZ.Frameworks.Extends;

using StackExchange.Redis;
using System.Reflection;
using RedisDemoConsole;

namespace RedisDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var conn = RedisHelper.GetRedisConn;
            IDatabase db = conn.GetDatabase();

            var dict = new Dictionary<string, PropertyInfo>();

            for (int i = 1; i < 10; i++)
            {
                var key = $"urn:PBS:Dynamic:CityPrice:" + i;

                var product = new Product
                {
                    ID = i,
                    Name = "产品名" + i,
                    Price = 10 + i,
                };
                //dict = product.GetType().GetProperties().ToDictionary(p => p.Name.ToLower(), p => p);

                var aa = product.GetType().GetProperties().ToDictionary(p => System.Text.Encoding.UTF8.GetBytes(p.Name.ToLower()), p => System.Text.Encoding.UTF8.GetBytes(p.GetValue(product, null).ToJson()));

                var keys = aa.Keys.ToArray();
                var values = aa.Values.ToArray();

                var hashEntries = new HashEntry[keys.Length];
                for (var a = 0; a < aa.Count; a++)
                {
                    hashEntries[a] = new HashEntry(keys[a], values[a]);
                }
                db.HashSet(key, hashEntries);
            }

            Console.WriteLine("success");

        }



    }
}
