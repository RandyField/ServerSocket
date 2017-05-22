using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketServer
{
    public class ServerSocket
    {
        //声明
        public Hashtable sessionTable = new Hashtable();//包括客户端会话
        private object sessionLock = new object();
        Socket serverSocket = null;
        Socket clientSocket = null;
        public int port { get; set; }
        public string host { get; set; }
        public bool IsRunning { get; set; }

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
        public void accept_receive(TextBox txtSocket,TextBox txtRecord)
        {
            try
            {
                while (IsRunning)
                {
                    //客户端socket
                    clientSocket = serverSocket.Accept();

                    

                    //保存socket
                    Session newSession = new Session(clientSocket);

                    lock (sessionLock)
                    {
                        this.sessionTable.Add(newSession.IP, newSession);
                    }


                    //Form1 fr = new Form1();
                    txtSocket.Text+= string.Format("{0}客户端{1}连接", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ((IPEndPoint)this.clientSocket.RemoteEndPoint).ToString());
                    txtSocket.Text+= "\r\n";
                    //接收数据
                    SocketConnection socketConnection = new SocketConnection(clientSocket);
                    socketConnection.addtxt(txtSocket, txtRecord);
                    socketConnection.ReceiveData();//接收数据       
                }
            }
            catch (Exception ex)
            {
            }

        }
    }
}
