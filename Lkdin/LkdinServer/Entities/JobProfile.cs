using System;
using System.Collections.Generic;

namespace LKDIN_Server.Domain
{
    public class JobProfile
    {
        public const int AbilityMinLength = 3;
        public const int AbilityMaxLength = 50;

        public const int DescriptionMinLength = 3;
        public const int DescriptionMaxLength = 250;

        public const int ImagePathMinLength = 3;
        public const int ImagePathMaxLength = 250;

        private List<string> abilities;
        private string description;
        private string imagePath;

        public List<string> Alities
        {
            get => this.abilities;
            set
            {
                this.abilities = value ?? throw new ArgumentNullException("La lista de habilidades no puede ser null");
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