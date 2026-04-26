using Agroreuse.Domain.Common;
using Agroreuse.Domain.Enums;

namespace Agroreuse.Domain.Entities
{
    /// <summary>
    /// Order entity representing a farmer's order
    /// </summary>
    public class Order : Entity
    {
        /// <summary>
        /// Foreign key to the seller (farmer user)
        /// </summary>
        public string SellerId { get; set; }

        /// <summary>
        /// Navigation property to the seller (ApplicationUser)
        /// </summary>
        public virtual ApplicationUser Seller { get; set; }

        /// <summary>
        /// Foreign key to Address
        /// </summary>
        public Guid AddressId { get; set; }

        /// <summary>
        /// Navigation property to Address
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// Foreign key to Category
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// Navigation property to Category
        /// </summary>
        public virtual Category Category { get; set; }
        /// <summary>
        /// Description of the order (optional)
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Quantity of the order
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Number of days (stored as string)
        /// </summary>
        public string NumberOfDays { get; set; }

        /// <summary>
        /// Status of the order
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// Date when the order was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Collection of images associated with the order (max 4)
        /// </summary>
        public virtual ICollection<OrderImage> Images { get; set; } = new List<OrderImage>();

        public Order() : base()
        {
            CreatedAt = DateTime.UtcNow;
            Status = OrderStatus.Pending;
        }

        public Order(
            string sellerId,
            Guid addressId,
            Guid categoryId,
            string? description,
            int quantity,
            string numberOfDays) : base()
        {
            SellerId = sellerId;
            AddressId = addressId;
            CategoryId = categoryId;
            Description = description ?? string.Empty;
            Quantity = quantity;
            NumberOfDays = numberOfDays;
            CreatedAt = DateTime.UtcNow;
            Status = OrderStatus.Pending;
        }

        public void Update(
            Guid addressId,
            Guid categoryId,
            int quantity,
            string numberOfDays,
            string? description,
            OrderStatus status)
        {
            AddressId = addressId;
            CategoryId = categoryId;
            Quantity = quantity;
            NumberOfDays = numberOfDays;
            Description = description ?? string.Empty;
            Status = status;
        }

        public void UpdateStatus(OrderStatus status)
        {
            Status = status;
        }

        public void AddImage(OrderImage image)
        {
            if (Images.Count >= 4)
                throw new InvalidOperationException("Cannot add more than 4 images to an order.");
            
            Images.Add(image);
        }

        public void ClearImages()
        {
            Images.Clear();
        }
    }
}
