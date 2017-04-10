using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisModelEntity
{
    /// <summary>
    /// 产品实体
    /// </summary>
    public class Product
    {
        /// <summary>
        /// 产品GUID
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 产品描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
    }
}
