using LKDIN_Server.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LkdinServer.Logic
{
    public class UserLogic
    {
        private List<User> users = new List<User>();

        public User CreateUser(string name, int age, List<string> professions, string country)
        {
            User newUser = null;

            if (!Exists(name))
            {
                newUser = new User()
                {
                    Name = name,
                    Age = age,
                    Professions = professions,
                    Country = country,
                    Profile = new JobProfile(),
                    Inbox = new List<Message>()
                };

                users.Add(newUser);

            }
            return newUser;
        }

        internal void AddMessage(Message newMessage, User receptor)
        {
            receptor.Inbox.Add(newMessage);
        }

        public User GetUserByName(string name)
        {
            User userToReturn = null;

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name == name)
                {
                    userToReturn = users[i]; 
                }
            }

            return userToReturn;
        }

        private bool Exists(string name)
        {
            return (GetUserByName(name) != null);
        }

        public void AssignJobProfile(string name, JobProfile jobProfile)
        {
            User userToAssign = GetUserByName(name);

            userToAssign.Profile = jobProfile;
        }

        public List<string> GetUsersName()
        {
            List<string> usersName = new List<string>();
            for (int i = 0; i < users.Count; i++)
            {
                usersName.Add(users[i].Name);
            }

            return usersName;
        }

        public string ShowMessages(string user, bool readMessages)
        {
            List<Message> messages = new List<Message>();
            string filteredMessages = "";

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name == user) 
                {
                    messages = users[i].Inbox;
                }
            }

            for (int i = 0; i < messages.Count; i++)
            {
                if (!messages[i].Read && !readMessages)
                {
                    filteredMessages += "DESTINATARIO: " + messages[i].Sender.Name + "\n" + messages[i].Content;
                    messages[i].Read = true;
                }
                else if (messages[i].Read && readMessages)
                {
                    filteredMessages += "DESTINATARIO: " + messages[i].Sender.Name + "\n" + messages[i].Content + "\n" +"\n";
                }
            }

            return filteredMessages;
        }

    }
}
