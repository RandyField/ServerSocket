using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    /// <summary>
    /// socket异步
    /// </summary>
    public class SocketConnection
    {
        public Hashtable sessionTable = new Hashtable();//包括客户端会话
        public Socket serverSocket { get; set; }
        public Socket clientSocket { get; set; }

        public string receivedContent { get; set; }
        byte[] MsgBuffer = new byte[4096];

        //public Byte[] MsgBuffer = null;
        private int totalLength = 0;
        public int CurrentBufferLength;

        bool disposeConnect = true;

        public SocketConnectionType Type { get; private set; }

        /// <summary>
        ///构造函数
        /// </summary>
        public SocketConnection(Socket serverSocket, Socket clientSocket)
        {
            this.serverSocket = serverSocket;
            this.clientSocket = clientSocket;
            this.Type = SocketConnectionType.Server;
        }

        public SocketConnection(Socket serverSocket, Socket clientSocket, Hashtable ht)
        {
            this.serverSocket = serverSocket;
            this.clientSocket = clientSocket;
            this.sessionTable = ht;
            this.Type = SocketConnectionType.Server;
        }

        public SocketConnection(Socket clientSocket)
        {
            this.clientSocket = clientSocket;
            this.Type = SocketConnectionType.Client;
        }


        #region 连接


        /// <summary>
        /// 连接-开始对远程主机连接的异步请求
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(IPAddress ip, int port)
        {
            //远程主机的 System.Net.IPAddress。
            //远程主机的端口号
            //requestCallback System.AsyncCallback 委托，它引用连接操作完成时要调用的方法
            //一个用户定义对象，其中包含连接操作的相关信息。操作完成时，此对象传递给了 requestCallback 委托。        
            this.clientSocket.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), this.serverSocket);
        }

        /// <summary>
        /// 连接操作完成时要调用的方法  可以连接为js中回调函数
        /// </summary>
        /// <param name="ar">异步操作的状态</param>
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndConnect(ar);
            }
            catch (SocketException ex)
            {

            }
        }

        #endregion


        #region 发送


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        public void Send(string data)
        {
            Send(System.Text.Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// 发送消息-byteData
        /// </summary>
        /// <param name="byteData"></param>
        private void Send(byte[] byteData)
        {
            try
            {
                //数据byte数组的长度
                int length = byteData.Length;

                // 将数据异步发送到连接的 System.Net.Sockets.Socket。
                this.clientSocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), this.clientSocket);

                ////头
                //byte[] head = BitConverter.GetBytes(length);

                ////待发送的数据
                //byte[] data = new byte[head.Length + byteData.Length];
                //Array.Copy(head, data, head.Length);
                //Array.Copy(byteData, 0, data, head.Length, byteData.Length);

                ////发送
                //this.serverSocket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), this.serverSocket);
            }
            catch (SocketException ex)
            {
                this.clientSocket.Dispose();
            }
        }

        /// <summary>
        /// 发送回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndSend(ar);
            }
            catch (SocketException ex)
            { }
        }

        #endregion


        #region 接收

        /// <summary>
        /// 接收数据
        /// </summary>
        public void ReceiveData()
        {
            if (MsgBuffer != null)
            {
                //从连接的 System.Net.Sockets.Socket 中异步接收数据。
                clientSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);
            }
        }

        /// <summary>
        /// 接收回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int REnd = clientSocket.EndReceive(ar);

                if (REnd > 0)
                {
                    disposeConnect = false;
                    byte[] data = new byte[REnd];
                    Array.Copy(MsgBuffer, 0, data, 0, REnd);

                    //收到结果                
                    string str = System.Text.Encoding.UTF8.GetString(data);

                    //在此处可以对data进行按需处理
                    Console.WriteLine(string.Format("客户端{0}传来消息:{1}", ((IPEndPoint)this.clientSocket.RemoteEndPoint).ToString(), str));
                    
                    //分发
                    foreach (Session session in this.sessionTable.Values)
                    {
                        
                        JObject obj = JObject.Parse(str);
                        if (obj["type"] != null)
                        {
                            SocketConnection socketConnection = new SocketConnection(session.ClientSocket);
                            socketConnection.Send(obj.ToString());
                        }
                    }

                    clientSocket.BeginReceive(MsgBuffer, 0, MsgBuffer.Length, 0, new AsyncCallback(ReceiveCallback), null);

                }
                else
                {
                    disposeConnect = true;
                    dispose();
                }
            }
            catch (SocketException ex)
            {
                dispose();
                disposeConnect = true;
            }
        }

        /// <summary>
        /// 释放资源
        /// 语言是C#，因为.net有垃圾回收机制，因此在实际开发中产生的托管资源都是系统自动释放完成
        /// 在关闭Form窗体是运行的系统并没有完全关闭。查找原因，应该是有资源没有被释放。
        /// 而socket套接字产生的资源恰好是非托管资源，此现象表明系统中有socket资源没有被完全释放掉。因此写了一个资源释放函数：
        /// </summary>
        private void dispose()
        {
            try
            {
                //this.clientSocket.Shutdown(SocketShutdown.Both);
                //this.clientSocket.Close();

                this.clientSocket.Shutdown(SocketShutdown.Both);
                this.clientSocket.Dispose();
                this.clientSocket.Close();
                this.clientSocket = null;
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

    }
}
