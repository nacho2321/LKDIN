using LKDIN_Server.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LkdinServer.Logic
{
    class UserLogic
    {
        private List<User> users = new List<User>();

        public void CreateUser(string name, string surname, int age, List<string> professions, string country)
        {
            User newUser = new User()
            {
                Name = name,
                Surname = surname,
                Age = age,
                Professions = professions,
                Country = country,
                Profile = null,
                Inbox = null
            };

            users.Add(newUser);
        }

    }
}
