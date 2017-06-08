using Newtonsoft.Json.Linq;
using SocketServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace Service
{
    class Program
    {
        static SerialPort com;
        static SerialPort com1;
        static ServerSocket ss = null;
        static byte[] readBuffer = new byte[5];
        static int nTotal = 5;
        //static Hashtable ht1 = new Hashtable();
        //static Hashtable ht2 = new Hashtable();

        //static Dictionary<DateTime, double> dic = new Dictionary<DateTime, double>();
        static List<dataModel> list1 = new List<dataModel>();
        static List<dataModel> list2 = new List<dataModel>();

        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {

                int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                string ip = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Ip"]);
                ss = new ServerSocket(port, ip);
                //int abc = 10000;
                //byte[] byteData = BitConverter.GetBytes(abc);
                Start();

                //获取配置文件COM口
                string strPort1 = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["COMPORT1"]);
                string strPort2 = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["COMPORT2"]);

                #region player1串口

                com = new SerialPort();

                //将处理 System.IO.Ports.SerialPort 对象的数据接收事件 为委托注册方法com_DataReceived
                com.DataReceived += new SerialDataReceivedEventHandler(com_DataReceived);

                //获取或设置串行波特率
                com.BaudRate = 9600;

                //获取或设置通信端口，包括但不限于所有可用的 COM 端口。
                com.PortName = strPort1;

                //获取或设置每个字节的标准数据位长度。
                com.DataBits = 8;

                //打开串口
                com.Open();

                #endregion

                #region palyer2串口

                com1 = new SerialPort();

                //将处理 System.IO.Ports.SerialPort 对象的数据接收事件 为委托注册方法com_DataReceived
                com1.DataReceived += new SerialDataReceivedEventHandler(com_DataReceived1);

                //获取或设置串行波特率
                com1.BaudRate = 9600;

                //获取或设置通信端口，包括但不限于所有可用的 COM 端口。
                com1.PortName = strPort2;

                //获取或设置每个字节的标准数据位长度。
                com1.DataBits = 8;

                //打开串口
                com1.Open();

                #endregion

                StartP1();
                StartP2();
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 串口数据接收1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int nRead = 0;
                while (nRead < nTotal)
                {
                    int count = com.BytesToRead;
                    if (count > nTotal)
                    {
                        com.Read(readBuffer, nRead, nTotal);
                        if (0xAA == readBuffer[0])
                        {
                            //转速模块
                            if (0x01 == readBuffer[1])
                            {
                                int highbit = readBuffer[2];
                                int sum = highbit * 256;
                                int lowbit = readBuffer[3];
                                double speed = (sum + lowbit) / 60.0 / 3.2;
                                list1.Clear();
                                dataModel model = new dataModel();
                                model.time = DateTime.Now;
                                model.speed = speed;
                                list1.Add(model);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Error(string.Format("{0} com_DataReceived读取串口数据发生异常，异常信息{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ex.ToString()));
            }
        }


        /// <summary>
        /// 串口数据接收2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void com_DataReceived1(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int nRead = 0;
                while (nRead < nTotal)
                {
                    int count = com1.BytesToRead;
                    if (count > nTotal)
                    {
                        com1.Read(readBuffer, nRead, nTotal);
                        if (0xAA == readBuffer[0])
                        {
                            //转速模块
                            if (0x01 == readBuffer[1])
                            {
                                int highbit = readBuffer[2];
                                int sum = highbit * 256;
                                int lowbit = readBuffer[3];
                                double speed = (sum + lowbit) / 60.0 / 3.2;
                                list2.Clear();
                                dataModel model = new dataModel();
                                model.time = DateTime.Now;
                                model.speed = speed;
                                list2.Add(model);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Helper.Logger.Error(string.Format("{0} com_DataReceived1读取串口数据发生异常，异常信息{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ex.ToString()));
            }
        }

        /// <summary>
        /// Player1发送数据
        /// </summary>
        static void ThreadSend1()
        {
            bool start = true;
            while (true)
            {

                Thread.Sleep(1000);
                //下面执行函数。
                if (list1 != null && list1.Count > 0)
                {
                    for (int i = 0; i < list1.Count; i++)
                    {
                        TimeSpan ts = DateTime.Now - list1[0].time;
                        if (ts.TotalSeconds > 1)
                        {
                            if (!start)
                            {
                                //start = true;
                                break;
                            }
                            else
                            {
                                //停止
                                int flag = 0;
                                SendData("player1", 0, 0);
                                start = false;
                            }

                        }
                        else
                        {
                            //开始
                            int flag = 1;
                            int sp = (int)Math.Round(list1[0].speed);
                            if (sp == 0)
                            {
                                SendData("player1", (int)Math.Round(list1[0].speed, MidpointRounding.AwayFromZero), 0);
                                start = true;
                            }
                            else
                            {
                                SendData("player1", (int)Math.Round(list1[0].speed, MidpointRounding.AwayFromZero), 1);
                                start = true;
                            }

                        }
                    }

                    //if (start)
                    //{
                    //    continue;
                    //}
                }
            }
        }

        /// <summary>
        /// Player2发送数据
        /// </summary>
        static void ThreadSend2()
        {
            bool start = true;
            while (true)
            {

                Thread.Sleep(1000);
                //下面执行函数。

                if (list2 != null && list2.Count > 0)
                {
                    for (int i = 0; i < list2.Count; i++)
                    {
                        TimeSpan ts = DateTime.Now - list2[0].time;
                        if (ts.TotalSeconds > 1)
                        {
                            if (!start)
                            {
                                //start = true;
                                break;
                            }
                            else
                            {
                                //停止
                                int flag = 0;
                                SendData("player2", 0, 0);
                                start = false;
                            }

                        }
                        else
                        {
                            //开始
                            int flag = 1;
                            int sp = (int)Math.Round(list2[0].speed);
                            if (sp == 0)
                            {
                                SendData("player2", (int)Math.Round(list2[0].speed, MidpointRounding.AwayFromZero), 0);
                                start = true;
                            }
                            else
                            {
                                SendData("player2", (int)Math.Round(list2[0].speed, MidpointRounding.AwayFromZero), 1);
                                start = true;
                            }
                        }
                    }

                    //if (start)
                    //{
                    //    continue;
                    //}
                }
            }
        }

        /// <summary>
        /// player1
        /// </summary>
        static void StartP1()
        {
            Thread thread = new Thread(new ThreadStart(ThreadSend1));
            thread.Start();
        }

        /// <summary>
        /// player2
        /// </summary>
        static void StartP2()
        {
            Thread thread = new Thread(new ThreadStart(ThreadSend2));
            thread.Start();
        }

        /// <summary>
        /// 发送 组织数据json
        /// </summary>
        /// <param name="nSpeed"></param>
        /// <param name="nFlag"></param>
        static void SendData(string player, int nSpeed, int nFlag)
        {
            JObject sendData = new JObject();
            sendData["player"] = player;
            sendData["speed"] = nSpeed;
            sendData["flag"] = nFlag;
            Send(sendData);
        }

        /// <summary>
        ///  发送
        /// </summary>
        /// <param name="content"></param>
        static void Send(JObject content)
        {
            Hashtable ht = ss.sessionTable;
            foreach (Session session in ht.Values)
            {
                if (session.ClientSocket.Connected == true)
                {
                    SocketConnection socketConnection = new SocketConnection(session.ClientSocket);
                    //Console.WriteLine(content.ToString());
                    socketConnection.SendBikeData(content.ToString());
                }
            }
        }


        /// <summary>
        ///  线程启动
        /// </summary>
        public static void Start()
        {
            //StartServer();
            Thread thread = new Thread(new ThreadStart(StartServer));
            thread.Start();
        }

        /// <summary>
        /// 启动socket监听
        /// </summary>
        public static void StartServer()
        {
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
            Console.WriteLine(string.Format("{0}  {1}:{2}端口已打开，监听中...", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ss.host, ss.port));
            ss.accept_receive();
        }

    }
}
