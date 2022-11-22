﻿using LkdinServerGrpc.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LkdinServerGrpc.Logic
{
    public class MessageLogic
    {
        private UserLogic userLogic;

        public MessageLogic(UserLogic userLogic)
        {
            this.userLogic = userLogic;
        }

        public void CreateMessage(User sender, User receptor, string content)
        {
            Message newMessage = new Message()
            {
                Sender = sender,
                Receptor = receptor,
                Content = content
            };
            this.userLogic.AddMessage(newMessage, receptor);
        }
    }
}
