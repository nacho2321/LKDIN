using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LkdinConnection
{
    public class Sender
    {
        const int protocolDataLength = 4; //TODO Pasar a archivo global accesible por los dos proyectos
        const int protocolCmdLength = 1; //TODO Pasar a archivo global accesible por los dos proyectos

        public void Send(Command order, Socket socket)
        {
            Send(order, "", socket);
        }

        public void Send(Command order, string message, Socket socket)
        {
            
            byte[] data = Encoding.UTF8.GetBytes(message);
            int dataLength = data.Length;
            
            int dataLengthBytes = dataLength.ToString().Length;
            int zerosToAdd = protocolDataLength - dataLengthBytes;

            string lengthOfMessage = "";
            for (int i = 0; i < zerosToAdd; i++)
            {
                lengthOfMessage += "0";
            }
            lengthOfMessage += dataLength.ToString();
            string header = (int)order + lengthOfMessage;

            byte[] headerData = Encoding.UTF8.GetBytes(header);

            BytesSender(headerData, socket); // Primero envio header con orden y largo de datos
            BytesSender(data, socket); // Luego envio los datos (el mensaje)

        }

        private void BytesSender(byte[] data, Socket socket)
        {
            int offset = 0;
            int size = data.Length;
            while (offset < size)
            {
                int sent = socket.Send(data, offset, size - offset, SocketFlags.None);
                if (sent == 0)
                {
                    throw new SocketException();
                }
                offset += sent;
            }
        }
    }
}

public enum Command
{
    CreateUser,
    CreateJobProfile,
    SendMessage,
    ReadMessages,
    GetUsersName,
    GetJobProfiles,
    ThrowException
}
