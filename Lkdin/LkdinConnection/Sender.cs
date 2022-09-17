using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LkdinConnection
{
    public class Sender
    {
        public void SendBytes(int order, string message, Socket socket)
        {
            byte[] data = Encoding.UTF8.GetBytes(order +"#" + message);
            socket.Send(data);
        }
    }
}
