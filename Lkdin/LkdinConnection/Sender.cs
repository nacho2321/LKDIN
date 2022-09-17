using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace LkdinConnection
{
    public class Sender
    {

        public void SendBytes(string message, Socket socket)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            socket.Send(data);
        }
    }
}
