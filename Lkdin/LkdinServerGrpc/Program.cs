using LkdinConnection;
using LkdinConnection.Logic;
using LkdinServerGrpc.Connection;
using LkdinServerGrpc.Logic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
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
            LogPublisher logPublisher = new LogPublisher();
            IModel channel = logPublisher.Setting();

            UserLogic userLogic = UserLogic.GetInstance();
            JobProfileLogic jobProfileLogic = JobProfileLogic.GetInstance();
            MessageLogic messageLogic = new MessageLogic(userLogic);


            ConnectionHandler serverConnection = new ConnectionHandler(userLogic, jobProfileLogic, messageLogic, sender, listener);

            Console.WriteLine("Iniciando Aplicacion Servidor...");

            Task.Run(() => serverConnection.Listen());

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
