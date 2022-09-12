using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LkdinServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            const int maxClients = 3;

            Console.WriteLine("Iniciando Aplicacion Servidor...!");
            var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);

            socketServer.Bind(localEndpoint);
            socketServer.Listen(2);

            int conexiones = 0;
            while (conexiones < maxClients)
            {
                var socketClient = socketServer.Accept();
                Console.WriteLine("Acepte un nuevo pedido de conexion");
                new Thread(() => ClientHandler(socketClient)).Start();
                conexiones++;
            }
            Console.WriteLine("No hay mas conexiones disponibles");
        }

        static void ClientHandler(Socket socketCliente)
        {
            Console.WriteLine("Esperando por mensaje de cliente...");
            try
            {
                bool detainedClient = false;
                while (!detainedClient)
                {
                    byte[] data = new byte[256];
                    int received = socketCliente.Receive(data);
                    if (received == 0)
                    {
                        detainedClient = true;
                        throw new SocketException();
                    }

                    string message = $"Cliente dice: {Encoding.UTF8.GetString(data)}";
                    Console.WriteLine(message);
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Cliente Desconectado");
            }
        }
    }
}
