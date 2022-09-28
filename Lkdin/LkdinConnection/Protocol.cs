using System;
using System.Collections.Generic;
using System.Text;

namespace LkdinConnection
{
    public static class Protocol
    {
        public static readonly int protocolDataLength = 4;
        public static readonly int protocolCmdLength = 1;

        public static readonly int FixedFileSize = 8;
        public static readonly int MaxPacketSize = 32768;

        public static long FileParts(long size)
        {
            var fileParts = size / MaxPacketSize;
            return fileParts * MaxPacketSize == size ? fileParts : fileParts + 1;
        }
    }
}
