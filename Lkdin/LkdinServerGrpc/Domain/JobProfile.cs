using LKDIN_Server.Exceptions;
using System;
using System.Collections.Generic;

namespace LKDIN_Server.Domain
{
    public class JobProfile
    {
        public const int NameMinLength = 2;
        public const int NameMaxLength = 50;

        public const int AbilityMinLength = 3;
        public const int AbilityMaxLength = 50;

        public const int DescriptionMinLength = 3;
        public const int DescriptionMaxLength = 250;

        public const int ImagePathMinLength = 3;
        public const int ImagePathMaxLength = 550;

        private string name;
        private List<string> abilities;
        private string description;
        private string imagePath;

        public string Name
        {
            get => this.name;
            set
            {
                if (value?.Length < NameMinLength || value?.Length > NameMaxLength)
                {
                    throw new DomainException($"El nombre del perfil de trabajo debe tener entre {NameMinLength} y {NameMaxLength} caracteres de largo");
                }
                this.name = value ?? throw new ArgumentNullException("El nombre no puede ser null");
            }
        }

        public string Description
        {
            get => this.description;
            set
            {
                if (value?.Length < DescriptionMinLength || value?.Length > DescriptionMaxLength)
                {
                    throw new DomainException($"El largo de la descripción debe estar entre {DescriptionMinLength} y {DescriptionMaxLength}");
                }
                this.description = value ?? throw new ArgumentNullException("La descripción no puede ser null");
            }
        }

        public List<string> Abilities
        {
            get => this.abilities;
            set
            {
                this.abilities = value ?? throw new ArgumentNullException("La lista de habilidades no puede ser null");
            }
        }

        public string ImagePath
        {
            get => this.imagePath;
            set
            {
                if (value?.Length < ImagePathMinLength || value?.Length > ImagePathMaxLength)
                {
                    throw new DomainException($"El largo de la ruta de la imagen debe estar entre {ImagePathMinLength} y {ImagePathMaxLength}");
                }
                this.imagePath = value ?? throw new ArgumentNullException("La ruta de la imagen no puede ser null");
            }
        }
    }
}