using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lkdin
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Aplicacion Cliente...");
            var socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            socketClient.Bind(localEndpoint);

            var remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);

            socketClient.Connect(remoteEndpoint);


            Console.WriteLine("Conectado con el servidor");

            while (true)
            {
                String mensaje = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(mensaje);
                socketClient.Send(data);
            }

            Console.ReadLine();
            Console.WriteLine("Voy a cerrar la conexion...");

            socketClient.Shutdown(SocketShutdown.Both);
            socketClient.Close();

            bool showMenu = true;
            while (showMenu)
            {
                showMenu = MainMenu();
            }
        }

        private static bool MainMenu()
        {
            Console.WriteLine("     ██████╗░██╗███████╗███╗░░██╗██╗░░░██╗███████╗███╗░░██╗██╗██████╗░░█████╗░");
            Console.WriteLine("     ██╔══██╗██║██╔════╝████╗░██║██║░░░██║██╔════╝████╗░██║██║██╔══██╗██╔══██╗");
            Console.WriteLine("     ██████╦╝██║█████╗░░██╔██╗██║╚██╗░██╔╝█████╗░░██╔██╗██║██║██║░░██║██║░░██║");
            Console.WriteLine("     ██╔══██╗██║██╔══╝░░██║╚████║░╚████╔╝░██╔══╝░░██║╚████║██║██║░░██║██║░░██║");
            Console.WriteLine("     ██████╦╝██║███████╗██║░╚███║░░╚██╔╝░░███████╗██║░╚███║██║██████╔╝╚█████╔╝");
            Console.WriteLine("     ╚═════╝░╚═╝╚══════╝╚═╝░░╚══╝░░░╚═╝░░░╚══════╝╚═╝░░╚══╝╚═╝╚═════╝░░╚════╝");
            Console.WriteLine(" ");
            Console.WriteLine("----------------------------------------------------------------------------- ");
            Console.WriteLine(" ");
            Console.WriteLine("                           |1|   OPCIÓN 1");
            Console.WriteLine("                           |2|   OPCIÓN 2");
            Console.WriteLine("                           |3|   OPCIÓN 3");
            Console.WriteLine(" ");
            Console.WriteLine("                           |0|    SALIR");

            switch (Console.ReadLine())
            {
                case "1":
                    return true;
                case "2":
                    return true;
                case "3":
                    return true;
                case "0":
                    return false;
                default:
                    return true;
            }
        }
    }
}
