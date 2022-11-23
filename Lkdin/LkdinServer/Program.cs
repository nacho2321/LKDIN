using LkdinConnection;
using LkdinConnection.Logic;
using LkdinServer.Connection;
using LkdinServer.Logic;
using System;
using System.Threading.Tasks;

namespace LkdinServer
{
    class Program
    {
        static Sender sender = new Sender();
        static Listener listener = new Listener();

        public static async Task Main(string[] args)
        {
            UserLogic userLogic = new UserLogic();
            JobProfileLogic jobProfileLogic = new JobProfileLogic(userLogic);
            MessageLogic messageLogic = new MessageLogic(userLogic);

            ConnectionHandler serverConnection = new ConnectionHandler(userLogic, jobProfileLogic, messageLogic, sender, listener);

            Console.WriteLine("Iniciando Aplicacion Servidor...");
            await serverConnection.Listen();
        }
    }
}