using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LkdinServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //const int maxConexiones = 3;

            Console.WriteLine("Iniciando Aplicacion Servidor...");
            var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);

            socketServer.Bind(localEndpoint);
            socketServer.Listen(1);

            //int conexiones = 0;
            //while (conexiones < maxConexiones)
            //{
            var socketClient = socketServer.Accept();

            Console.WriteLine("Acepte un nuevo pedido de conexion");
            while (socketClient.Connected)
            {
                byte[] data = new byte[256];
                socketClient.Receive(data);
                string mensaje = Encoding.UTF8.GetString(data);
                Console.WriteLine(mensaje);
            }

            new Thread(() => ManejarCliente(socketClient)).Start();
            // conexiones++;
            // }

            //Console.WriteLine("No hay conexiones disponibles");
            //Console.ReadLine();

        }

        static void ManejarCliente(Socket socketCliente)
        {
            while (socketCliente.Connected)
            {
                Thread.Sleep(1000);
            }

            Console.WriteLine("Cliente Desconectado...");
        }
    }
}
