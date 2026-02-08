using Agroreuse.Domain.Common;

namespace Agroreuse.Domain.Entities
{
    /// <summary>
    /// Represents an image associated with an order
    /// </summary>
    public class OrderImage : Entity
    {
        /// <summary>
        /// Relative path to the image stored in wwwroot
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Foreign key to Order
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Navigation property to Order
        /// </summary>
        public virtual Order Order { get; set; }

        public OrderImage() : base()
        {
        }

        public OrderImage(string imagePath, Guid orderId) : base()
        {
            ImagePath = imagePath;
            OrderId = orderId;
        }
    }
}
