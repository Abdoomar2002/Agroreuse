using Agroreuse.Domain.Common;

namespace Agroreuse.Domain.Entities
{
    /// <summary>
    /// City entity related to a Government
    /// </summary>
    public class City : Entity
    {
        public string Name { get; set; }

        /// <summary>
        /// Foreign key to Government
        /// </summary>
        public Guid GovernmentId { get; set; }

        /// <summary>
        /// Navigation property to Government
        /// </summary>
        public virtual Government Government { get; set; }

        public City() : base()
        {
        }

        public City(string name, Guid governmentId) : base()
        {
            Name = name;
            GovernmentId = governmentId;
        }
    }
}
