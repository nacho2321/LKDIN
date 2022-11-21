using LkdinConnection;
using LkdinConnection.Logic;
using LkdinServer.Connection;
using LkdinServerGrpc.Logic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace LkdinServerGrpc
{
    public class Program
    {       
        static Sender sender = new Sender();
        static Listener listener = new Listener();

        public static async Task Main(string[] args)
        {
            UserLogic userLogic = new UserLogic();
            JobProfileLogic jobProfileLogic = new JobProfileLogic(userLogic);
            MessageLogic messageLogic = new MessageLogic(userLogic);
            FileLogic fileLogic = new FileLogic();

            ConnectionHandler serverConnection = new ConnectionHandler(userLogic, jobProfileLogic, messageLogic, sender, listener, fileLogic);

            Console.WriteLine("Iniciando Aplicacion Servidor...");

            await serverConnection.Listen();

            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
