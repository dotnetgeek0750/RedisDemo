using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RedisModelEntity;
using StackExchange.Redis;
using CtripSZ.Frameworks.Extends;

namespace RedisWebSite.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult List()
        {
            var conn = RedisHelper.GetRedisConn;
            IDatabase db = conn.GetDatabase();

            var key = $"urn:Product:10086";
            var ss = db.StringGetRange(key, 1, 2);


           

            return Content("");
        }

        // GET: Product/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(Product product)
        {
            try
            {
                // TODO: Add insert logic here


                var conn = RedisHelper.GetRedisConn;
                IDatabase db = conn.GetDatabase();

                product.ID = Guid.NewGuid();

                var key = $"urn:Product:{product.ID}";
                db.StringSet(key, product.ToJson());

                //var dict = new Dictionary<string, PropertyInfo>();

                //for (int i = 1; i < 10; i++)
                //{
                //    var key = $"urn:PBS:Dynamic:CityPrice:" + i;

                //    var product = new Product
                //    {
                //        ID = i,
                //        Name = "产品名" + i,
                //        Price = 10 + i,
                //    };
                //    //dict = product.GetType().GetProperties().ToDictionary(p => p.Name.ToLower(), p => p);

                //    var aa = product.GetType().GetProperties().ToDictionary(p => System.Text.Encoding.UTF8.GetBytes(p.Name.ToLower()), p => System.Text.Encoding.UTF8.GetBytes(p.GetValue(product, null).ToJson()));

                //    var keys = aa.Keys.ToArray();
                //    var values = aa.Values.ToArray();

                //    var hashEntries = new HashEntry[keys.Length];
                //    for (var a = 0; a < aa.Count; a++)
                //    {
                //        hashEntries[a] = new HashEntry(keys[a], values[a]);
                //    }
                //    db.HashSet(key, hashEntries);
                //}
                //Console.WriteLine("success");


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Product/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
