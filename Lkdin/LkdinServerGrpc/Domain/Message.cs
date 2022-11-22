﻿using System;

namespace LkdinServerGrpc.Domain
{
    public class Message
    {
        private User sender;
        private User receptor;
        private bool read;
        private string content;

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

        public string Content
        {
            get => this.content;
            set
            {
                this.content = value ?? throw new ArgumentNullException("El mensaje no puede ser null");
            }
        }

        public void ReadMessage()
        {
            this.read = true;
        }
    }
}