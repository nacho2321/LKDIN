using System;

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

        private string ability;
        private string description;
        private string imagePath;

        public string Ability
        {
            get => this.ability;
            set
            {
                if (value?.Length < AbilityMinLength || value?.Length > AbilityMaxLength)
                {
                    throw new DomainException($"El largo de la habilidad debe estar entre {AbilityMinLength} y {AbilityMaxLength}");
                }
                this.ability = value ?? throw new ArgumentNullException("La habilidad no puede ser null");
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