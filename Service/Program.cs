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
        static ServerSocket ss = null;
        static byte[] readBuffer = new byte[5];
        static int nTotal = 5;
        static void Main(string[] args)
        {
            try
            {
                int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                string ip = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["Ip"]);
                ss = new ServerSocket(port, ip);
                Start();

                //获取配置文件COM口
                string strPort = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["COMPORT"]);
                com = new SerialPort();

                //将处理 System.IO.Ports.SerialPort 对象的数据接收事件 为委托注册方法com_DataReceived
                com.DataReceived += new SerialDataReceivedEventHandler(com_DataReceived);

                //获取或设置串行波特率
                com.BaudRate = 9600;

                //获取或设置通信端口，包括但不限于所有可用的 COM 端口。
                com.PortName = strPort;

                //获取或设置每个字节的标准数据位长度。
                com.DataBits = 8;

                //打开串口
                com.Open();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 串口数据接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                int nRead = 0;
                while (nRead < nTotal)
                {
                    // 摘要:
                    //     获取接收缓冲区中数据的字节数。
                    //
                    // 返回结果:
                    //     接收缓冲区中数据的字节数。
                    //
                    int count = com.BytesToRead;
                    if (count > nTotal)
                    {
                        com.Read(readBuffer, nRead, nTotal);
                        //nRead = nTotal;

                        if (0x55 == readBuffer[0])
                        {
                            int nStartFlag = readBuffer[2];
                            int nSpeed = readBuffer[3];
                            SendData(nSpeed, nStartFlag);
                        }
                    }
                    else
                    {
                        com.Read(readBuffer, nRead, count);
                        nRead += count;
                    }
                }

                if (0x55 == readBuffer[0])
                {
                    int nStartFlag = readBuffer[2];
                    int nSpeed = readBuffer[3];
                    SendData(nSpeed, nStartFlag);
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="nSpeed"></param>
        /// <param name="nFlag"></param>
        static void SendData(int nSpeed, int nFlag)
        {
            JObject sendData = new JObject();
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
                    Console.WriteLine(content.ToString());
                    socketConnection.Send(content.ToString());
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
