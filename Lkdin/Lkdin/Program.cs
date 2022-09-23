using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using LKDIN_Server.Domain;
using LkdinConnection;

namespace Lkdin
{
    class Program
    {
        static Sender sender = new Sender();
        static Listener listener = new Listener();
        static readonly SettingsManager settingsMngr = new SettingsManager();
        static Socket socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando Aplicacion Cliente...");

            string ipServer = settingsMngr.ReadSettings(ClientConfig.serverIPconfigkey);
            string ipClient = settingsMngr.ReadSettings(ClientConfig.clientIPconfigkey);
            int serverPort = int.Parse(settingsMngr.ReadSettings(ClientConfig.serverPortconfigkey));

            var localEndPoint = new IPEndPoint(IPAddress.Parse(ipClient), 0);
            socketClient.Bind(localEndPoint);
            var serverEndpoint = new IPEndPoint(IPAddress.Parse(ipServer), serverPort);

            try
            {
                socketClient.Connect(serverEndpoint);
                Console.WriteLine("Conectado con el servidor");

                bool showMenu = true;
                while (showMenu)
                {
                    showMenu = MainMenu();
                }
            }
            catch (SocketException ex)
            {

                Console.WriteLine("No se ha podido conectar con el servidor, reinicie la aplicación e intente nuevamente");
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
            Console.WriteLine("                           |0|   SALIR");

            switch (Console.ReadLine())
            {
                case "1":
                    CreateUser();
                    return true;
                case "2":
                    JobProfileMenu();
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

            sender.Send(Command.CreateUser, userData, socketClient);
            Console.WriteLine(listener.RecieveData(socketClient)[1]);
        }

        private static void JobProfileMenu()
        {
            string jobProfileData = "";
            Console.WriteLine("█▀█ █▀▀ █▀█ █▀▀ █ █░░   █▀▄ █▀▀   ▀█▀ █▀█ ▄▀█ █▄▄ ▄▀█ ░░█ █▀█");
            Console.WriteLine("█▀▀ ██▄ █▀▄ █▀░ █ █▄▄   █▄▀ ██▄   ░█░ █▀▄ █▀█ █▄█ █▀█ █▄█ █▄█");
            string user = UsersMenu("asignar un perfil de trabajo:");

            Console.WriteLine("Descripción:");
            jobProfileData += Console.ReadLine() + "-";
            Console.WriteLine("Ubicación de la foto de perfil:");
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

            sender.Send(Command.CreateJobProfile, jobProfileData, socketClient);
            Console.WriteLine(listener.RecieveData(socketClient)[1]);
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
            string users = "";
            string message = "";
            Console.WriteLine("ENVIAR MENSAJES");
            users += UsersMenu("Elija el usuario que va a mandar un mensaje:") + "-";
            users += UsersMenu("Elija el usuario que va a recibir el mensaje:") + "-";
            Console.WriteLine("Escriba su mensaje: ");
            message = Console.ReadLine();

            sender.Send(Command.SendMessage, users+message, socketClient);
            Console.WriteLine(listener.RecieveData(socketClient)[1]);
        }

        private static void Inbox()
        {
            Console.WriteLine("BANDEJA DE ENTRADA");
            string user = UsersMenu("Elija el usuario que desea ver su bandeja de entrada:");
            repeat:
            Console.WriteLine("|1|   Ver mensajes nuevos" +
            "\n|2|   Ver mensajes leídos");

            string option = Console.ReadLine();
            if (option == "2")
            {
                sender.Send(Command.ReadMessages, user + "-" + "readMessages", socketClient);
            }
            else if (option != "1" && option != "2")
            {
                Console.WriteLine("\n Debe ingresar solamenente 1 o 2");
                goto repeat;
            }
            else
            {
                sender.Send(Command.ReadMessages, user + "-" + "newMessages", socketClient);
            }

            Console.WriteLine(listener.RecieveData(socketClient)[1]);
            Console.WriteLine(listener.RecieveData(socketClient)[1]);
        }

        private static string UsersMenu(string action)
        {
            sender.Send(Command.GetUsersName, socketClient);
            string users = listener.RecieveData(socketClient)[1];

            repeat:
            Console.WriteLine(action);
            Console.WriteLine(users);

            string userSelected = Console.ReadLine();

            if (!users.Contains(userSelected))
            {
                Console.WriteLine("Verifique los datos ingresados");
                goto repeat;
            }

            return userSelected;
        }

    }
}
