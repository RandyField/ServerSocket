using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        public int port { get; set; }
        public string ip { get; set; }

        ServerSocket ss = null;

        /// <summary>
        /// 窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            ReadConfig();
            ss = new ServerSocket(this.port, this.ip);
            Start();     
      
        }    
     
        /// <summary>
        /// 读取配置文件
        /// </summary>
        public void ReadConfig()
        {
            this.port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
            this.ip = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Ip"]);
        }

        /// <summary>
        /// 线程启动
        /// </summary>
        public void Start()
        {
            Thread thread = new Thread(new ThreadStart(StartServer));
            thread.Start();
        }

        /// <summary>
        /// 启动sockie监听
        /// </summary>
        public void StartServer()
        {
            string content;
            string client;
            if (ss == null)
            {
                int port = 8099;
                string host = "127.0.0.1";
                ss = new ServerSocket(port, host);
            }
            ss.IsRunning = true;
            ss.socket();
            ss.bind();
            ss.Listen();
            txtSocket.Text += string.Format("{0}:{1}端口已打开，监听中...", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ss.port);
            txtSocket.Text += "\r\n";
            ss.accept_receive(txtSocket,txtRecord);
        }


        /// <summary>
        /// 发送,服务端向客服端发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string content = textBox1.Text;
            Send(content);
            txtRecord.Text += string.Format("{0}  服务端：{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), textBox1.Text);
            txtRecord.Text += "\r\n";
            textBox1.Text = "";
            textBox1.Clear();
        }

        /// <summary>
        /// 发送数据到不同的客户端 默认发给所有连接的客户端
        /// </summary>
        public void Send(string content)
        {
            Hashtable ht = ss.sessionTable;
            foreach (Session session in ht.Values)
            {
                if (session.ClientSocket.Connected==true)
                {
                    SocketConnection socketConnection = new SocketConnection(session.ClientSocket);
                    string str = content;
                    socketConnection.Send(str);
                }

            }
        }
    }
}
