using LkdinConnection;
using LkdinServer.Connection;
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
            ConnectionHandler serverConnection = new ConnectionHandler();
            Console.WriteLine("Iniciando Aplicacion Servidor...");
            serverConnection.Listen();
        }

    }
}
