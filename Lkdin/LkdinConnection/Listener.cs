using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace LkdinConnection
{
    public class Listener
    {
        const int protocolDataLength = 4; //TODO Pasar a archivo global accesible por los dos proyectos
        const int protocolCmdLength = 1; //TODO Pasar a archivo global accesible por los dos proyectos

        public string[] RecieveData(Socket socket)
        {
            int headerLength = protocolCmdLength + protocolDataLength;
            byte[] header = BytesReciever(headerLength, socket);

            string headerToString = Encoding.UTF8.GetString(header);
            int length = Int32.Parse(headerToString.Substring(protocolCmdLength));
            byte[] data = BytesReciever(length, socket);

            string order = headerToString.Substring(0, protocolCmdLength);
            string[] ret = new string[2];
            ret[0] = order;
            ret[1] = Encoding.UTF8.GetString(data);

            return ret;
        }

        private byte[] BytesReciever(int length, Socket socket)
        {
            byte[] response = new byte[length];
            int offset = 0;
            while (offset < length) 
            {
                int received = socket.Receive(response, offset, length - offset, SocketFlags.None);
                if (received == 0)
                {
                    throw new SocketException();
                }
                offset += received;
            }
            return response;
        }
    }
}
