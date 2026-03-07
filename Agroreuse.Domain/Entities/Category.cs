using Agroreuse.Domain.Common;

namespace Agroreuse.Domain.Entities
{
    /// <summary>
    /// Category entity with name and image
    /// </summary>
    public class Category : Entity
    {
        public string Name { get; set; }
        
        /// <summary>
        /// Relative path to category image stored in wwwroot
        /// </summary>
        public string? ImagePath { get; set; }

        public Category() : base()
        {
        }

        public Category(string name, string? imagePath = null) : base()
        {
            Name = name;
            ImagePath = imagePath;
        }
    }
}
