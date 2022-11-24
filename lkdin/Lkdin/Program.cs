using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LkdinConnection;
using LkdinConnection.Exceptions;
using LkdinConnection.Logic;

namespace Lkdin
{
    class Program
    {
        static Sender sender = new Sender();
        static Listener listener = new Listener();
        static readonly SettingsManager settingsMngr = new SettingsManager();
        static TcpClient tcpClient;

        static int specialCharactersUsed = 0;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Iniciando Aplicacion Cliente...");

            string ipServer = settingsMngr.ReadSettings(ClientConfig.serverIPconfigkey);
            int serverPort = int.Parse(settingsMngr.ReadSettings(ClientConfig.serverPortconfigkey));

            string ipClient = settingsMngr.ReadSettings(ClientConfig.clientIPconfigkey);
            int clientPort = int.Parse(settingsMngr.ReadSettings(ClientConfig.clientPortconfigkey));

            var localEndPoint = new IPEndPoint(IPAddress.Parse(ipClient), clientPort);

            tcpClient = new TcpClient(localEndPoint);

            await tcpClient.ConnectAsync(IPAddress.Parse(ipServer), serverPort).ConfigureAwait(false);

            try
            {
                await using (var netStream = tcpClient.GetStream())
                {
                    bool showMenu = true;
                    Console.WriteLine("Conectado con el servidor");

                    while (showMenu)
                    {
                        showMenu = await MainMenu(netStream);
                    }
                }

                tcpClient.Close();
            }
            catch (Exception ex)
            {

                Console.WriteLine("No se ha podido conectar con el servidor, reinicie la aplicación e intente nuevamente");
            }
        }

        private static async Task<bool> MainMenu(NetworkStream netStream)
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
                    await CreateUser(netStream);
                    return true;
                case "2":
                    await JobProfileMenu(netStream);
                    return true;
                case "3":
                    await ConsultSpecificProfile(netStream);
                    return true;
                case "4":
                    await ShowJobProfiles(netStream);
                    return true;
                case "5":
                    await MessageMenu(netStream);
                    return true;
                case "0":
                    tcpClient.Close();
                    return false;
                default:
                    Console.WriteLine("Verifique el dato ingresado...");
                    return true;
            }
        }

        private static async Task CreateUser(NetworkStream netStream)
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
                await sender.Send(Command.CreateUser, userData, netStream);
                Console.WriteLine((await listener.ReceiveData(netStream))[1]);
            }
            specialCharactersUsed = 0;
        }

        private static async Task JobProfileMenu(NetworkStream netStream)
        {
            if (await UsersLoaded(netStream))
            {
                string jobProfileData = "";
                Console.WriteLine("█▀█ █▀▀ █▀█ █▀▀ █ █░░   █▀▄ █▀▀   ▀█▀ █▀█ ▄▀█ █▄▄ ▄▀█ ░░█ █▀█");
                Console.WriteLine("█▀▀ ██▄ █▀▄ █▀░ █ █▄▄   █▄▀ ██▄   ░█░ █▀▄ █▀█ █▄█ █▀█ █▄█ █▄█");
                jobProfileData += await UsersMenu("Elija el usuario al que le desea asignar un perfil de trabajo:", netStream) + "-";
                jobProfileData += await AssignJobProfilesMenu("Elija el perfil de trabajo: ", netStream);

                await sender.Send(Command.AssignJobProfile, jobProfileData, netStream);
                Console.WriteLine((await listener.ReceiveData(netStream))[1]);
            }
            else
            {
                Console.WriteLine("Para realizar esta acción primero debe de tener un usuario creado");
            }
        }

        private static async Task MessageMenu(NetworkStream netStream)
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
                    await SendMessage(netStream);
                    break;
                case "2":
                    await Inbox(netStream);
                    break;
                case "0":
                    await MainMenu(netStream);
                    break;
                default:
                    Console.WriteLine("Verifique el número ingresado...");
                    await MessageMenu(netStream);
                    break;
            }
        }

        private static async Task SendMessage(NetworkStream netStream)
        {
            if (await UsersLoaded(netStream))
            {
                string users = "";
                string message = "";
                Console.WriteLine("ENVIAR MENSAJES");
                users += await UsersMenu("Elija el usuario que va a mandar un mensaje:", netStream) + "-";
                users += await UsersMenu("Elija el usuario que va a recibir el mensaje:", netStream) + "-";
                Console.WriteLine("Escriba su mensaje: ");
                message = Console.ReadLine();

                if (!ContainsSpecialCharacters(message, 0))
                {
                    await sender.Send(Command.SendMessage, users + message, netStream);
                    Console.WriteLine((await listener.ReceiveData(netStream))[1]);
                }
                specialCharactersUsed = 0;
            }
            else
            {
                Console.WriteLine("Para realizar esta acción primero debe de tener un usuario creado");
            }
        }

        private static async Task Inbox(NetworkStream netStream)
        {
            if (await UsersLoaded(netStream))
            {
                Console.WriteLine("BANDEJA DE ENTRADA");
                string user = await UsersMenu("Elija el usuario que desea ver su bandeja de entrada:", netStream);
                repeat:
                Console.WriteLine("|1|   Ver mensajes nuevos" +
                "\n|2|   Ver mensajes leídos");

                string option = Console.ReadLine();
                if (option == "2")
                {
                    await sender.Send(Command.ReadMessages, user + "-" + "readMessages", netStream);
                }
                else if (option != "1" && option != "2")
                {
                    Console.WriteLine("\n Debe ingresar solamenente 1 o 2");
                    goto repeat;
                }
                else
                {
                    await sender.Send(Command.ReadMessages, user + "-" + "newMessages", netStream);
                }

                Console.WriteLine((await listener.ReceiveData(netStream))[1]);
                Console.WriteLine((await listener.ReceiveData(netStream))[1]);
            }
            else
            {
                Console.WriteLine("Para realizar esta acción primero debe de tener un usuario creado");
            }
        }

        private static async Task<string> UsersMenu(string action, NetworkStream netStream)
        {
            await sender.Send(Command.GetUsersName, netStream);
            string data = (await listener.ReceiveData(netStream))[1];
            List<string> users = data.Split(';').ToList();
            repeat:

            Console.WriteLine(action);
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine("|" + i + "|" + users[i]);
            }

            string userSelected = Console.ReadLine();

            if (!Regex.IsMatch(userSelected, @"^[0-9]+$") || Int32.Parse(userSelected) >= users.Count || Int32.Parse(userSelected) < 0)
            {
                Console.WriteLine("Verifique los datos ingresados");
                goto repeat;
            }

            return users[Int32.Parse(userSelected)];
        }

        private static async Task<string> AssignJobProfilesMenu(string action, NetworkStream netStream)
        {
            await sender.Send(Command.GetJobProfiles, netStream);
            string dataJobProfiles = (await listener.ReceiveData(netStream))[1];
            List<string> jobProfiles = dataJobProfiles.Split(';').ToList();

            repeat:
            Console.WriteLine(action);

            if (jobProfiles[0] != "")
            {
                for (int i = 0; i < jobProfiles.Count; i++)
                {
                    Console.WriteLine("|" + i + "|" + jobProfiles[i]);
                }
            }

            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine("|" + jobProfiles.Count + "| AGREGAR NUEVO PERFIL DE TRABAJO");

            string data = Console.ReadLine();

            if (!Regex.IsMatch(data, @"^[0-9]+$") || Int32.Parse(data) > jobProfiles.Count || Int32.Parse(data) < 0 || (Int32.Parse(data) == 0 && jobProfiles[0] == ""))
            {
                Console.WriteLine("Verifique los datos ingresados");
                goto repeat;
            }
            else if (Int32.Parse(data) == (jobProfiles.Count))
            {
                return await CreateJobProfile(netStream);
            }

            return jobProfiles[Int32.Parse(data)];
        }

        private static async Task<string> CreateJobProfile(NetworkStream netStream)
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
            jobProfileData += FileLogic.GetName(path) + "-";
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
                try
                {
                    await sender.SendFile(path, netStream);
                    await sender.Send(Command.CreateJobProfile, jobProfileData, netStream);
                    Console.WriteLine((await listener.ReceiveData(netStream))[1]);
                }
                catch (FileException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                specialCharactersUsed = 0;
                goto repeat;
            }

            return name;
        }

        private static async Task ConsultSpecificProfile(NetworkStream netStream)
        {
            if (await UsersLoaded(netStream))
            {
                string user = await UsersMenu("Elija el usuario que desea ver el perfil de trabajo:", netStream);
                await sender.Send(Command.GetSpecificProfile, user, netStream);

                string recievedData = (await listener.ReceiveData(netStream))[1];

                string[] splittedData = recievedData.Split('-');

                if (splittedData[splittedData.Length - 1] != "")
                {
                    await listener.ReceiveData(netStream);
                }
                Console.WriteLine(FormatSpecificProfile(recievedData));
            }
            else
            {
                Console.WriteLine("Para realizar esta acción primero debe de tener un usuario creado");
            }
        }

        private static async Task ShowJobProfiles(NetworkStream netStream)
        {
            await sender.Send(Command.GetJobProfiles, netStream);
            Console.WriteLine((await listener.ReceiveData(netStream))[1]);
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

        private static async Task<bool> UsersLoaded(NetworkStream netStream)
        {
            await sender.Send(Command.GetUsersName, netStream);
            string data = (await listener.ReceiveData(netStream))[1];

            return data != "";
        }

        private static string FormatSpecificProfile(string rawProfileData)
        {
            string[] splittedData = rawProfileData.Split('-');
            string filePath = splittedData[splittedData.Length - 1] != "" ? FileLogic.GetPath(splittedData[2]) : "----";

            string profile = "NOMBRE: " + splittedData[0] + "\nDESCRIPCIÓN: " + splittedData[1] + "\nFOTO DE PERFIL: " + filePath;

            return profile;
        }
    }
}