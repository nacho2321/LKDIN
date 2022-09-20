using LKDIN_Server.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LkdinServer.Logic
{
    public class UserLogic
    {
        private List<User> users = new List<User>();

        public void CreateUser(string name, int age, List<string> professions, string country)
        {
            User newUser = new User()
            {
                Name = name,
                Age = age,
                Professions = professions,
                Country = country,
                Profile = null,
                Inbox = null
            };

            users.Add(newUser);
        }

        internal void AddMessage(Message newMessage, User receptor)
        {
            receptor.Inbox.Add(newMessage);
        }

        public User GetUserByName (string name)
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

        public void AssignJobProfile(string name, JobProfile jobProfile)
        {
            User userToAssign = GetUserByName(name);

            userToAssign.Profile = jobProfile;
        }

        public List<User> GetUsers()
        {
            return users;
        }

    }
}
