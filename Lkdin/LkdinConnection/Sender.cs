using LkdinConnection.Exceptions;
using LkdinConnection.Logic;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LkdinConnection
{
    public class Sender
    {
        private readonly int protocolDataLength;
        private readonly int maxPacketSize;

        private readonly FileStreamHandler fileStreamHandler;
        
        public Sender()
        {
            this.protocolDataLength = Protocol.protocolDataLength;
            this.maxPacketSize = Protocol.MaxPacketSize;

            this.fileStreamHandler = new FileStreamHandler();
        }

        public async Task Send(Command order, NetworkStream netStream)
        {
            await Send(order, "", netStream);
        }

        public async Task Send(Command order, string message, NetworkStream netStream)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            int dataLength = data.Length;

            byte[] headerData = this.HeaderEncoder(order, dataLength);

            await BytesSender (headerData, netStream);    // Primero envia header con orden y largo de datos
            await BytesSender (data, netStream);          // Luego envia los datos (el mensaje)
        }

        public async Task SendFile(string path, NetworkStream netStream)
        {
            if (FileLogic.Exists(path))
            {
                string fileName = FileLogic.GetName(path);
                byte[] headerData = this.HeaderEncoder(Command.SendFile, fileName.Length);
                byte[] convertedfileName = Encoding.UTF8.GetBytes(fileName);

                long fileSize = FileLogic.GetFileSize(path);
                byte[] convertedfileSize = BitConverter.GetBytes(fileSize);

                await BytesSender(headerData, netStream);          // Envia header con orden y largo del nombre del archivo
                await BytesSender(convertedfileName, netStream);   // Envia nombre del archivo
                await BytesSender(convertedfileSize, netStream);   // Envia tamaño del archivo
                await FileStreamSenderAsync(fileSize, path, netStream); // Envia archivo
            }
            else
            {
                throw new FileException("El archivo no existe");
            }
        }

        private byte[] HeaderEncoder(Command order, int messageLength)
        {
            int dataLengthBytes = messageLength.ToString().Length;
            int zerosToAdd = protocolDataLength - dataLengthBytes;

            string lengthOfMessage = "";

            for (int i = 0; i < zerosToAdd; i++)
            {
                lengthOfMessage += "0";
            }

            lengthOfMessage += messageLength.ToString();
            string header = (int)order + lengthOfMessage;

            return Encoding.UTF8.GetBytes(header);
        }

        private async Task BytesSender(byte[] data, NetworkStream netStream)
        {
            int size = data.Length;
            await netStream.WriteAsync(data, 0, size).ConfigureAwait(false);
        }

        private async Task FileStreamSenderAsync(long fileSize, string path, NetworkStream netStream)
        {
            long fileParts = Protocol.FileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = fileStreamHandler.Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = fileStreamHandler.Read(path, offset, maxPacketSize);
                    offset += maxPacketSize;
                }

                await BytesSender(data, netStream);

                currentPart++;
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
    AssignJobProfile,
    SendFile,
    GetSpecificProfile,
    ThrowException
}
