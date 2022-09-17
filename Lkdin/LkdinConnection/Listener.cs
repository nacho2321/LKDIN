using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace LkdinConnection
{
    public class Listener
    {
        public void Handler(Socket socket)
        {
            try
            {
                bool detainedClient = false;
                while (!detainedClient)
                {
                    byte[] data = new byte[256];
                    int received = socket.Receive(data);
                    if (received == 0)
                    {
                        detainedClient = true;
                        throw new SocketException();
                    }

                    string message = Encoding.UTF8.GetString(data);
                    Console.WriteLine(message);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Desconectado");
            }
        }
    }
}
