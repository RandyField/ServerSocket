using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer
{
    /// <summary>
    /// 客户端session
    /// </summary>
    public class Session
    {
        public Socket ClientSocket { get; set; }//客户端socket

        public string IP;//客户端ip

        /// <summary>
        /// 获取ip地址
        /// </summary>
        /// <returns></returns>
        public string GetIPString()
        {
            string result=((IPEndPoint)ClientSocket.RemoteEndPoint).ToString();
            return result;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clientSocket"></param>
        public Session(Socket clientSocket)
        {
            this.ClientSocket = clientSocket;
            this.IP = GetIPString();
        }
    }
}
