using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StringIncrement计数器
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379"))
            {
                string NewsCount = "news_count";
                //默认是访问db0数据库，可以通过方法参数指定数字访问不同的数据库
                IDatabase db = redis.GetDatabase();
                //给Key增加1
                string val = db.StringGet(NewsCount);
                label2.Text = val.ToString();
            }
        }




        private async void button1_Click(object sender, EventArgs e)
        {
            string NewsCount = "news_count";
            using (ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379"))
            {
                //默认是访问db0数据库，可以通过方法参数指定数字访问不同的数据库
                IDatabase db = redis.GetDatabase();
                //给Key增加1
                await db.StringIncrementAsync(NewsCount, 1);
                //读取Key的值
                var count = await db.StringGetAsync(NewsCount);

                label2.Text = count.ToString();
            }
        }
    }
}
