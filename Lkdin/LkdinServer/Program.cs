using LkdinConnection;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LkdinServer
{
    class Program
    {
        static Sender sender = new Sender();
        static Listener listener = new Listener();

        public static void Main(string[] args)
        {

            Console.WriteLine("Iniciando Aplicacion Servidor...!");
            // Generico
            var socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000); // Generico - IP y Puerto dinamico

            socketServer.Bind(localEndpoint);// Generico
            // SOLO SERVER //////////////////////////////
            const int maxClients = 3;
            socketServer.Listen(maxClients); // recibe por parametro la cantidad maxima de conexiones permitidas

            int conexiones = 0;
            while (conexiones < maxClients)
            {
                var socketClient = socketServer.Accept();
                Console.WriteLine("Acepte un nuevo pedido de conexion");
                new Thread(() => listener.Handler(socketClient)).Start();
                conexiones++;
            }
            Console.WriteLine("No hay mas conexiones disponibles");
            /////////////////////////////////////////////
        }

    }
}
