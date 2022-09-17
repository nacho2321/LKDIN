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
        // Generico
        static Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Sender sender = new Sender();

        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Aplicacion Cliente...");
            // Generico
            var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0); // Generico - IP y Puerto dinamico
            socketClient.Bind(localEndpoint); // Generico

            var remoteEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000); // Leer del archivo - es el socket del server
            
            // SOLO CLIENTE
            socketClient.Connect(remoteEndpoint);
            Console.WriteLine("Conectado con el servidor");
            ///////////////
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
                    return true;
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
            string userData = "";
            Console.WriteLine("█▀▀ █▀█ █▀▀ ▄▀█ █▀▀ █ █▀█ █▄░█   █▀▄ █▀▀   █░█ █▀ █░█ ▄▀█ █▀█ █ █▀█ █▀");
            Console.WriteLine("█▄▄ █▀▄ ██▄ █▀█ █▄▄ █ █▄█ █░▀█   █▄▀ ██▄   █▄█ ▄█ █▄█ █▀█ █▀▄ █ █▄█ ▄█");
            
            Console.WriteLine("Nombre:");
            userData += Console.ReadLine() + "-";

            Console.WriteLine("Apellido:");
            userData += Console.ReadLine() + "-";

            Console.WriteLine("Edad:");
            string age = Console.ReadLine();
            while (!Regex.IsMatch(age, @"^[0-9]+$"))
            {
                Console.WriteLine("Debe ingresar solamente números");
                age = Console.ReadLine();
            }
            
            userData += age + "-";

            Console.WriteLine("Profesiones:");
            bool addProfessions = true;
            while (addProfessions)
            {
                Console.WriteLine("Agregue una Profesión:");
                userData += Console.ReadLine() + ";";
                repeat:
                    Console.WriteLine("\n                         |1|    AGREGAR MÁS PROFESIONES");
                    Console.WriteLine("\n                         |0|    DEJAR DE AGREGAR PROFESIONES");

                    string option = Console.ReadLine();
                    if (option == "0")
                    {
                        userData = userData.Remove(userData.Length - 1, 1);
                        userData += "-";
                        addProfessions = false;
                    }
                    else if (option != "0" && option != "1")
                    {
                        Console.WriteLine("\n Debe ingresar solamenente 1 o 0");
                        goto repeat;
                    }
            }

            Console.WriteLine("País:");
            userData += Console.ReadLine() + "-";

            sender.SendBytes(Command.CreateUser, userData, socketClient);
            Console.WriteLine("\n USUARIO CREADO CORRECTAMENTE"); //TODO - Respuesta según servidor
        }

        private static void CreateJobProfile()
        {
            string jobProfileData = "";
            Console.WriteLine("█▀█ █▀▀ █▀█ █▀▀ █ █░░   █▀▄ █▀▀   ▀█▀ █▀█ ▄▀█ █▄▄ ▄▀█ ░░█ █▀█");
            Console.WriteLine("█▀▀ ██▄ █▀▄ █▀░ █ █▄▄   █▄▀ ██▄   ░█░ █▀▄ █▀█ █▄█ █▀█ █▄█ █▄█");
            Console.WriteLine("Descripción:");
            jobProfileData += Console.ReadLine() + "-";
            Console.WriteLine("Ubiación de la foto de perfil:");
            jobProfileData += Console.ReadLine() + "-";

            Console.WriteLine("Habilidades:");
            bool addAbilities = true;
            while (addAbilities)
            {
                Console.WriteLine("Agregue una Habilidad:");
                jobProfileData += Console.ReadLine() + ";";
                Console.WriteLine("\n                         |0|    DEJAR DE AGREGAR HABILIDADES");

                string option = Console.ReadLine();
                if (option == "0")
                {
                    jobProfileData = jobProfileData.Remove(jobProfileData.Length - 1, 1);
                    jobProfileData += "-";
                    addAbilities = false;
                }
            }

            sender.SendBytes(Command.CreateJobProfile, jobProfileData, socketClient);
            Console.WriteLine("\n PERFIL DE TRABAJO CREADO CORRECTAMENTE"); //TODO - Respuesta según servidor
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
            sender.SendBytes(Command.SendMessage, Console.ReadLine(), socketClient);
            Console.WriteLine("\n MENSAJE ENVIADO CORRECTAMENTE");//TODO - Respuesta según servidor
        }

        private static void Inbox()
        {
            Console.WriteLine("BANDEJA DE ENTRADA");

            sender.SendBytes(Command.SendMessage, socketClient);
            //TODO - Respuesta según servidor
        }
    }
}
