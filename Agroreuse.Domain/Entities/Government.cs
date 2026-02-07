using Agroreuse.Domain.Common;

namespace Agroreuse.Domain.Entities
{
    /// <summary>
    /// Government entity (Governorate/Province)
    /// </summary>
    public class Government : Entity
    {
        public string Name { get; set; }

        /// <summary>
        /// Collection of cities within this government
        /// </summary>
        public virtual ICollection<City> Cities { get; set; } = new List<City>();

        public Government() : base()
        {
        }

        public Government(string name) : base()
        {
            Name = name;
        }
    }
}
