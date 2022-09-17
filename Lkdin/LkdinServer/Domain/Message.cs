using System;

namespace LKDIN_Server.Domain
{
    public class Message
    {

        private User sender;
        private User receptor;
        private bool read;

        public User Sender
        {
            get => this.sender;
            set
            {
                this.sender = value ?? throw new ArgumentNullException("El emisor del mensaje no puede ser null");
            }
        }

        public User Receptor
        {
            get => this.receptor;
            set
            {
                this.receptor = value ?? throw new ArgumentNullException("El receptor del mensaje no puede ser null");
            }
        }

        public bool Read
        {
            get => this.read;
            set
            {
                this.read = value;
            }
        }

        public void ReadMessage()
        {
            this.read = true;
        }

    }
}