using System;
using System.Collections.Generic;

namespace LKDIN_Server.Domain
{
    public class User
    {
        public const int NameMinLength = 2;
        public const int NameMaxLength = 50;
        public const int MaxAge = 99;
        public const int MinAge = 18;

        private string name;
        private int age;
        private List<string> professions;
        private string country;
        private JobProfile profile;
        private List<Message> inbox;

        public string Name
        {
            get => this.name;
            set
            {
                if (value?.Length < NameMinLength || value?.Length > NameMaxLength)
                {
                    throw new DomainException($"Su nombre debe estar entre {NameMinLength} y {NameMaxLength}");
                }
                this.name = value ?? throw new ArgumentNullException("El nombre no puede ser null");
            }
        }

        public int Age
        {
            get => this.age;
            set
            {
                if (value > MaxAge || value < MinAge)
                {
                    throw new DomainException($"Su edad debe estar entre los {MinAge} y los {MaxAge} años");
                }
                this.age = value;
            }
        }

        public List<string> Professions
        {
            get => this.professions;
            set
            {
                this.professions = value ?? throw new ArgumentNullException("La lista de profesiones no puede ser null");
            }
        }

        public string Country
        {
            get => this.country;
            set
            {
                if (value?.Length < NameMinLength || value?.Length > NameMaxLength)
                {
                    throw new DomainException($"Su país debe estar entre {NameMinLength} y {NameMaxLength}");
                }
                this.surname = value ?? throw new ArgumentNullException("El país no puede ser null");
            }
        }

        public JobProfile Profile
        {
            get => this.profile;
            set => this.profile = value;
        }

        public List<Message> Inbox
        {
            get => this.inbox;
            set => this.inbox = value;
        }
    }
}

