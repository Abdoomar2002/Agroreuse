using Agroreuse.Domain.Common;

namespace Agroreuse.Domain.Entities
{
    /// <summary>
    /// Address entity with one-to-one nullable relationship to ApplicationUser
    /// </summary>
    public class Address : Entity
    {
        /// <summary>
        /// Foreign key to Government
        /// </summary>
        public Guid GovernmentId { get; set; }

        /// <summary>
        /// Navigation property to Government
        /// </summary>
        public virtual Government Government { get; set; }

        /// <summary>
        /// Foreign key to City
        /// </summary>
        public Guid CityId { get; set; }

        /// <summary>
        /// Navigation property to City
        /// </summary>
        public virtual City City { get; set; }

        /// <summary>
        /// Detailed address description (street, building, etc.)
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// Foreign key to ApplicationUser (nullable - one-to-one relationship)
        /// </summary>
        public string? ApplicationUserId { get; set; }

        /// <summary>
        /// Navigation property to ApplicationUser
        /// </summary>
        public virtual ApplicationUser? ApplicationUser { get; set; }

        public Address() : base()
        {
        }

        public Address(Guid governmentId, Guid cityId, string details) : base()
        {
            GovernmentId = governmentId;
            CityId = cityId;
            Details = details;
        }
    }
}
