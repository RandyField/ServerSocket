using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;

namespace socketApi.Controllers
{
    public class WSChatController : ApiController
    {
        /// <summary>
        /// HttpContext.AcceptWebSocketRequest创建WebSocket连接
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Get()
        {
            //判断请求是否是websocket请求
            if (HttpContext.Current.IsWebSocketRequest)
            {
                //接收websocket请求-委托
                HttpContext.Current.AcceptWebSocketRequest(ProcessWSChat);
            }

            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);  
        }

        private async Task ProcessWSChat(AspNetWebSocketContext arg)
        {
            //实例化websocket
            WebSocket socket = arg.WebSocket;
            while (true)
            {
                //新建buffer
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);

                //监听客户端 有则接收数据，存入buffer
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                if (socket.State == WebSocketState.Open)
                {
                    string message = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                    string returnMessage = "你刚刚发送的消息已经达到服务器，服务器发消息告诉你一声，返回您发送的消息 :" + message + " 时间:" + DateTime.Now.ToLongTimeString();
                    buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(returnMessage));
                    
                    //发送给客户端
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    break;
                }
            }
        }  
    }
}
