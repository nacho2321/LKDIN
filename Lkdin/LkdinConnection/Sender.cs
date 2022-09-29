using LkdinConnection.Exceptions;
using LkdinConnection.Logic;
using System.Net.Sockets;
using System.Text;

namespace LkdinConnection
{
    public class Sender
    {
        private readonly int protocolDataLength;
        private readonly int maxPacketSize;
        private readonly int fixedFileSize;

        private readonly FileLogic fileLogic;
        private readonly FileStreamHandler fileStreamHandler;
        
        public Sender()
        {
            this.protocolDataLength = Protocol.protocolDataLength;
            this.maxPacketSize = Protocol.MaxPacketSize;
            this.fixedFileSize = Protocol.FixedFileSize;

            this.fileLogic = new FileLogic();
            this.fileStreamHandler = new FileStreamHandler();
        }

        public void Send(Command order, Socket socket)
        {
            Send(order, "", socket);
        }

        public void Send(Command order, string message, Socket socket)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            int dataLength = data.Length;

            byte[] headerData = this.HeaderEncoder(order, dataLength);

            BytesSender(headerData, socket);    // Primero envia header con orden y largo de datos
            BytesSender(data, socket);          // Luego envia los datos (el mensaje)
        }

        public void SendFile(string path, Socket socket)
        {
            if (fileLogic.Exists(path))
            {
                string fileName = this.fileLogic.GetName(path);
                byte[] headerData = this.HeaderEncoder(Command.SendFile, fileName.Length);
                byte[] convertedfileName = Encoding.UTF8.GetBytes(fileName);

                long fileSize = this.fileLogic.GetFileSize(path);
                int zerosToAdd = fixedFileSize - fileSize.ToString().Length; 
                string fileSizeConverted = null; 

                for (int i = 0; i < zerosToAdd; i++)
                {
                    fileSizeConverted += "0"; 
                }
                fileSizeConverted += fileSize.ToString();

                byte[] convertedfileSize = Encoding.UTF8.GetBytes(fileSizeConverted);

                BytesSender(headerData, socket);          // Envia header con orden y largo del nombre del archivo
                BytesSender(convertedfileName, socket);   // Envia nombre del archivo
                BytesSender(convertedfileSize, socket);   // Envia tamaño del archivo
                FileStreamSender(fileSize, path, socket); // Envia archivo
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

        private void FileStreamSender(long fileSize, string path, Socket socket)
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

                BytesSender(data, socket);

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
