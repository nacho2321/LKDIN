using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LkdinServer.Connection
{
    class ConnectionHandler
    {
        const int maxClients = 3; // Se tiene que llamar desde archivo de configuracion

        private Socket socket;

        public ConnectionHandler()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000); // Se tiene que llamar desde archivo de configuracion (IP & puerto)
            this.socket.Bind(localEndpoint);
        }

        public void Listen()
        {
            this.socket.Listen(maxClients);

            int conexiones = 0;
            while (conexiones < maxClients)
            {
                var socketClient = this.socket.Accept();

                Console.WriteLine("Nueva conexión establecida");
                new Thread(() => this.Handler(socketClient)).Start();
                conexiones++;
            }
            Console.WriteLine("Limite de conexiones alcanzado");
        }

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
                Console.WriteLine("Cliente desconectado");
            }

        }

    }
}

enum Command
{
    createUser,
    createJobProfile,
}
