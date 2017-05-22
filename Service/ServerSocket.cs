using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    public class ServerSocket
    {
        //声明
        public Hashtable sessionTable = new Hashtable();//包括客户端会话
        private object sessionLock = new object();

        //服务端socket
        Socket serverSocket = null;

        //客户端socket
        Socket clientSocket = null;

        public int port { get; set; }
        public string host { get; set; }
        public bool IsRunning { get; set; }

        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="host">ip</param>
        public ServerSocket(int port, string host)
        {
            this.port = port;
            this.host = host;
        }

        /// <summary>
        /// socket()--创建socket
        /// </summary>
        public void socket()
        {
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }


        /// <summary>
        /// bind()-绑定socket和端口号
        /// </summary>
        public void bind()
        {
            //int port = 8099;
            //string host = "127.0.0.1";
            //ip转换
            IPAddress ip = IPAddress.Parse(this.host);

            //server address
            IPEndPoint ipe = new IPEndPoint(ip, this.port);

            serverSocket.Bind(ipe);
        }

        /// <summary>
        /// listen()-监听端口
        /// </summary>
        public void Listen()
        {
            try
            {
                serverSocket.Listen(50);
            }
            catch (SocketException ex)
            {

            }
        }

        /// <summary>
        /// accept()-接收来自客户端的连接请求 receive-从socket中读取字符
        /// </summary>
        public void accept_receive()
        {
            try
            {
                while (IsRunning)
                {
                    //连接的客户端socket
                    clientSocket = serverSocket.Accept();

                    //保存客户端socket
                    Session newSession = new Session(clientSocket);

                    lock (sessionLock)
                    {
                        this.sessionTable.Add(newSession.IP, newSession);
                    }
                    Console.WriteLine(string.Format("{0}客户端{1}连接", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ((IPEndPoint)this.clientSocket.RemoteEndPoint).ToString()));
                    SocketConnection socketConnection = new SocketConnection(serverSocket, clientSocket, sessionTable);
                    socketConnection.ReceiveData();//接收数据   
                    
                }
            }
            catch (SocketException ex)
            {
            }
        }
    }
}
