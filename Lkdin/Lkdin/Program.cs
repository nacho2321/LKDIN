using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using LkdinConnection;

namespace Lkdin
{
    class Program
    {
        static Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Sender sender = new Sender();

        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Aplicacion Cliente...");
            var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
            socketClient.Bind(localEndpoint);

            var remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);

            socketClient.Connect(remoteEndpoint);

            Console.WriteLine("Conectado con el servidor");

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
            Console.WriteLine("                           |1|   CREAR USUARIO");
            Console.WriteLine("                           |2|   CREAR PERFIL DE TRABAJO DE USUARIO");
            Console.WriteLine("                           |3|   AÑADIR FOTO A PERFIL DE TRABAJO");
            Console.WriteLine("                           |4|   MOSTRAR PERFILES DE TRABAJO EXISTENTES");
            Console.WriteLine("                           |5|   MENSAJES");
            Console.WriteLine("\n                         |0|    SALIR");

            switch (Console.ReadLine())
            {
                case "1":
                    CreateUser();
                    return true;
                case "2":
                    CreateJobProfile();
                    return true;
                case "3":
                    return true;
                case "4":
                    return true;;
                case "5":
                    MessageMenu();
                    return true;
                case "0":
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                    return false;
                default:
                    Console.WriteLine("Verifique el número ingresado...");
                    return true;
            }
        }

        private static void CreateUser()
        {
            string userData = "01#";
            Console.WriteLine("█▀▀ █▀█ █▀▀ ▄▀█ █▀▀ █ █▀█ █▄░█   █▀▄ █▀▀   █░█ █▀ █░█ ▄▀█ █▀█ █ █▀█ █▀");
            Console.WriteLine("█▄▄ █▀▄ ██▄ █▀█ █▄▄ █ █▄█ █░▀█   █▄▀ ██▄   █▄█ ▄█ █▄█ █▀█ █▀▄ █ █▄█ ▄█");
            Console.WriteLine("Nombre:");
            userData += Console.ReadLine() + "#";
            Console.WriteLine("Apellido:");
            userData += Console.ReadLine() + "#";
            Console.WriteLine("Edad:");
            string age = Console.ReadLine();

            while (!Regex.IsMatch(age, @"^[0-9]+$"))
            {
                Console.WriteLine("Debe ingresar solamente números");
                age = Console.ReadLine();
            }
            userData += age + "#";

            Console.WriteLine("Profesiones:");
            bool addProfessions = true;
            while (addProfessions)
            {
                Console.WriteLine("Agregue una Profesión:");
                userData += Console.ReadLine() + ";";
                Console.WriteLine("\n                         |0|    DEJAR DE AGREGAR PROFESIONES");

                string option = Console.ReadLine();
                if (option == "0")
                {
                    userData = userData.Remove(userData.Length - 1, 1);
                    userData += "#";
                    addProfessions = false;
                } 
            }

            Console.WriteLine("País:");
            sender.SendBytes(Console.ReadLine(), socketClient);

            Console.WriteLine("\n USUARIO CREADO CORRECTAMENTE");
        }

        private static void CreateJobProfile()
        {
            Console.WriteLine("█▀█ █▀▀ █▀█ █▀▀ █ █░░   █▀▄ █▀▀   ▀█▀ █▀█ ▄▀█ █▄▄ ▄▀█ ░░█ █▀█");
            Console.WriteLine("█▀▀ ██▄ █▀▄ █▀░ █ █▄▄   █▄▀ ██▄   ░█░ █▀▄ █▀█ █▄█ █▀█ █▄█ █▄█");
            Console.WriteLine("Descripción:");
            sender.SendBytes(Console.ReadLine(), socketClient);
            Console.WriteLine("Ubiación de la foto de perfil:");
            sender.SendBytes(Console.ReadLine(), socketClient);

            Console.WriteLine("Habilidades:");
            bool addAbilities = true;
            while (addAbilities)
            {
                Console.WriteLine("Agregue una Habilidad:");
                sender.SendBytes(Console.ReadLine(), socketClient);
                Console.WriteLine("\n                         |0|    DEJAR DE AGREGAR HABILIDADES");

                string option = Console.ReadLine();
                if (option == "0") addAbilities = false;
            }

            Console.WriteLine("\n PERFIL DE TRABAJO CREADO CORRECTAMENTE");
        }

        private static void MessageMenu() 
        {
            Console.WriteLine("█▀▄▀█ █▀▀ █▄░█ █▀ ▄▀█ ░░█ █▀▀ █▀");
            Console.WriteLine("█░▀░█ ██▄ █░▀█ ▄█ █▀█ █▄█ ██▄ ▄█");
            Console.WriteLine("|1|   ENVIAR MENSAJE" +
                "\n|2|   VER MENSAJES" +
                "\n|0|   VOLVER AL MENÚ PRINCIPAL");
            Console.WriteLine(" ");

            switch (Console.ReadLine())
            {
                case "1":
                    SendMessage();
                    break;
                case "2":
                    Inbox();
                    break;
                case "0":
                    MainMenu();
                    break;
                default:
                    Console.WriteLine("Verifique el número ingresado...");
                    MessageMenu();
                    break;
            }
        }

        private static void SendMessage()
        {
            Console.WriteLine("ENVIAR MENSAJES");
            sender.SendBytes(Console.ReadLine(), socketClient);
            Console.WriteLine("\n MENSAJE ENVIADO CORRECTAMENTE");
        }

        private static void Inbox()
        {
            Console.WriteLine("BANDEJA DE ENTRADA");
        }
    }
}
