using LKDIN_Server.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LkdinServer.Logic
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
