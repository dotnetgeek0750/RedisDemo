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

namespace ListProducer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var mail = txtMail.Text.Trim();
            using (ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379"))
            {
                IDatabase db = redis.GetDatabase();//默认是访问db0数据库，可以通过方法参数指定数字访问不同的数据库
                var res = await db.ListLeftPushAsync("RedisListMail", mail);
                MessageBox.Show(res.ToString());
            }
        }
    }
}
