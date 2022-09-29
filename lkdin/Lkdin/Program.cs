using System;
using System.Collections.Generic;
using System.IO;
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
        static int specialCharactersUsed = 0;

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
            Console.WriteLine("                           |3|   MOSTRAR PERFIL DE TRABAJO ESPECÍFICO");
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
                    ConsultSpecificProfile();
                    return true;
                case "4":
                    ShowJobProfiles();
                    return true;
                case "5":
                    MessageMenu();
                    return true;
                case "0":
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                    return false;
                default:
                    Console.WriteLine("Verifique el dato ingresado...");
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
            specialCharactersUsed++;
            Console.WriteLine("Edad:");
            string age = Console.ReadLine();
            while (!Regex.IsMatch(age, @"^[0-9]+$"))
            {
                Console.WriteLine("Debe ingresar solamente números");
                age = Console.ReadLine();
            }
            userData += age + "-";
            specialCharactersUsed++;
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
                specialCharactersUsed++;
            }

            Console.WriteLine("País:");
            userData += Console.ReadLine() + "-";
            specialCharactersUsed++;

            if (!ContainsSpecialCharacters(userData, specialCharactersUsed))
            {
                sender.Send(Command.CreateUser, userData, socketClient);
                Console.WriteLine(listener.ReceiveData(socketClient)[1]);
            }
            specialCharactersUsed = 0;
        }

        private static void JobProfileMenu()
        {
            if (UsersLoaded())
            {
                string jobProfileData = "";
                Console.WriteLine("█▀█ █▀▀ █▀█ █▀▀ █ █░░   █▀▄ █▀▀   ▀█▀ █▀█ ▄▀█ █▄▄ ▄▀█ ░░█ █▀█");
                Console.WriteLine("█▀▀ ██▄ █▀▄ █▀░ █ █▄▄   █▄▀ ██▄   ░█░ █▀▄ █▀█ █▄█ █▀█ █▄█ █▄█");
                jobProfileData += UsersMenu("Elija el usuario al que le desea asignar un perfil de trabajo:") + "-";
                jobProfileData += AssignJobProfilesMenu("Elija el perfil de trabajo: ");

                sender.Send(Command.AssignJobProfile, jobProfileData, socketClient);
                Console.WriteLine(listener.ReceiveData(socketClient)[1]);
            }
            else
            {
                Console.WriteLine("Para realizar esta acción primero debe de tener un usuario creado");
            }
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
			if (UsersLoaded())
			{
                string users = "";
                string message = "";
                Console.WriteLine("ENVIAR MENSAJES");
                users += UsersMenu("Elija el usuario que va a mandar un mensaje:") + "-";
                users += UsersMenu("Elija el usuario que va a recibir el mensaje:") + "-";
                Console.WriteLine("Escriba su mensaje: ");
                message = Console.ReadLine();

                if (!ContainsSpecialCharacters(message, 0))
                {
                    sender.Send(Command.SendMessage, users + message, socketClient);
                    Console.WriteLine(listener.ReceiveData(socketClient)[1]);
                }
                specialCharactersUsed = 0;
            }
            else
            {
                Console.WriteLine("Para realizar esta acción primero debe de tener un usuario creado");
            }
        }

        private static void Inbox()
        {
            if (UsersLoaded())
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

                Console.WriteLine(listener.ReceiveData(socketClient)[1]);
                Console.WriteLine(listener.ReceiveData(socketClient)[1]);
            }
            else
            {
                Console.WriteLine("Para realizar esta acción primero debe de tener un usuario creado");
            }
        }

        private static string UsersMenu(string action)
        {
            sender.Send(Command.GetUsersName, socketClient);
            string data = listener.ReceiveData(socketClient)[1];
            List<string> users = data.Split(';').ToList();
            repeat:

            Console.WriteLine(action);
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine("|"+ i + "| " + users[i]);
            }

            string userSelected = Console.ReadLine();

            if (!Regex.IsMatch(userSelected, @"^[0-9]+$") || Int32.Parse(userSelected) >= users.Count || Int32.Parse(userSelected) < 0)
            {
                Console.WriteLine("Verifique los datos ingresados");
                goto repeat;
            }

            return users[Int32.Parse(userSelected)];
        }

        private static string AssignJobProfilesMenu(string action)
        {
            sender.Send(Command.GetJobProfiles, socketClient);
            string dataJobProfiles = listener.ReceiveData(socketClient)[1];
            List<string> jobProfiles = dataJobProfiles.Split(';').ToList();

            repeat:
            Console.WriteLine(action);

            if (jobProfiles[0] != "")
            {
                for (int i = 0; i < jobProfiles.Count; i++)
                {
                    Console.WriteLine("|" + i + "| " + jobProfiles[i]);
                }
            }

            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("|" + jobProfiles.Count +"| AGREGAR NUEVO PERFIL DE TRABAJO");

            string data = Console.ReadLine();

            if (!Regex.IsMatch(data, @"^[0-9]+$") || Int32.Parse(data) > jobProfiles.Count|| Int32.Parse(data) < 0 || (Int32.Parse(data) == 0 && jobProfiles[0] == ""))
            {
                Console.WriteLine("Verifique los datos ingresados");
                goto repeat;
            }
            else if (Int32.Parse(data) == (jobProfiles.Count ))
            {
                return CreateJobProfile();
            }

            return jobProfiles[Int32.Parse(data)];
        }

        private static string CreateJobProfile()
        {
            repeat:
            Console.WriteLine("CREACIÓN DE PERFILES DE TRABAJO");
            string name = "";
            string jobProfileData = "";
            Console.WriteLine("Nombre:");
            jobProfileData += Console.ReadLine() + "-";
            name = jobProfileData;
            specialCharactersUsed++;
            Console.WriteLine("Descripción:");
            jobProfileData += Console.ReadLine() + "-";
            specialCharactersUsed++;
            Console.WriteLine("Ubicación de la foto de perfil:");
            string path = Console.ReadLine();
            jobProfileData += path + "-";
            specialCharactersUsed++;
            Console.WriteLine("Habilidades:");
            bool addAbilities = true;
            while (addAbilities)
            {
                Console.WriteLine("Agregue una Habilidad:");
                jobProfileData += Console.ReadLine() + ";";
                Console.WriteLine("PRESIONE: ");
                Console.WriteLine("\n                         |Cualquier tecla|    SEGUIR AGREGANDO HABILIDADES");
                Console.WriteLine("\n                         |0|    DEJAR DE AGREGAR HABILIDADES");

                string option = Console.ReadLine();
                if (option == "0")
                {
                    jobProfileData = jobProfileData.Remove(jobProfileData.Length - 1, 1);
                    jobProfileData += "-";
                    addAbilities = false;
                }
                specialCharactersUsed++;
            }

            if (!ContainsSpecialCharacters(jobProfileData, specialCharactersUsed))
            {
                specialCharactersUsed = 0;
                sender.SendFile(path, socketClient);
                string fileName = new FileInfo(path).Name;
                string newPath = Environment.CurrentDirectory.Replace("Lkdin\\Lkdin", "Lkdin\\LkdinServer") + "\\" + fileName;

                if (File.Exists(newPath))
                {
                    sender.Send(Command.CreateJobProfile, jobProfileData.Replace(path, newPath), socketClient);
                    Console.WriteLine(listener.ReceiveData(socketClient)[1]);
                }
                else 
                {
                    Console.WriteLine("EL ARCHIVO INGRESADO NO PUDO SER CARGADO, PRUEBE CON OTRO"); 
                    Console.WriteLine(" ");

                    goto repeat;
                }
            }
            else {
                specialCharactersUsed = 0;
                goto repeat;
            }

            return name;
        }

        private static void ConsultSpecificProfile()
        {
            if (UsersLoaded())
            {
                string user = UsersMenu("Elija el usuario que desea ver el perfil de trabajo:");
                sender.Send(Command.GetSpecificProfile, user, socketClient);
                Console.WriteLine(listener.ReceiveData(socketClient)[1]);
            }
            else
            {
                Console.WriteLine("Para realizar esta acción primero debe de tener un usuario creado");
            }
        }

        private static void ShowJobProfiles()
        {
            sender.Send(Command.GetJobProfiles, socketClient);
            Console.WriteLine(listener.ReceiveData(socketClient)[1]);
        }

        private static bool ContainsSpecialCharacters(string data, int charactersUsedBySystem)
        {
            bool contains = false;
            int freq = data.Where(x => (x == '|') || (x == ';') || (x == '-')).Count();

            if (freq > charactersUsedBySystem)
            {
                contains = true;
                Console.WriteLine("Los datos que usted ingrese no pueden contener: ; | -");
            }

            return contains;
        }

        private static bool UsersLoaded()
        {
            sender.Send(Command.GetUsersName, socketClient);
            string data = listener.ReceiveData(socketClient)[1];
            
            return data != "";
        }
    }
}
