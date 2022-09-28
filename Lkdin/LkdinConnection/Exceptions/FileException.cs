using System;
using System.Runtime.Serialization;

namespace LkdinConnection.Exceptions
{
    [Serializable]
    internal class FileException : Exception
    {
        public FileException()
        {
        }

        public FileException(string message) : base(message)
        {
        }

        public FileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}