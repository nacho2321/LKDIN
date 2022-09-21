using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LkdinConnection
{
    public class Sender
    {
        public void SendBytes(Command order, string message, Socket socket)
        {
            byte[] data = Encoding.UTF8.GetBytes((int)order + "#" + message);
            socket.Send(data);
        }

        public void SendBytes(Command order, Socket socket)
        {
            SendBytes(order, "", socket);
        }

    }
}
public enum Command
{
    CreateUser,
    CreateJobProfile,
    SendMessage,
    ReadMessages,
    GetUsers,
    ThrowException
}
