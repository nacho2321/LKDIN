using LkdinConnection;
using LkdinServer.Connection;
using LkdinServer.Logic;
using System;

namespace LkdinServer
{
    class Program
    {
        static Sender sender = new Sender();
        static Listener listener = new Listener();

        public static void Main(string[] args)
        {
            UserLogic userLogic = new UserLogic();
            ConnectionHandler serverConnection = new ConnectionHandler(userLogic);
            Console.WriteLine("Iniciando Aplicacion Servidor...");
            serverConnection.Listen();
        }

    }
}
