using Agroreuse.Application.Common.Interface;
using Agroreuse.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Agroreuse.Infrastructure.Persistence
{
    public class ArgoreuseContext : IdentityDbContext<ApplicationUser>, IArgoreuseContext
    {
        public ArgoreuseContext(DbContextOptions<ArgoreuseContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Government> Governments { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ContactUs> ContactUsMessages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderImage> OrderImages { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ImagePath).HasMaxLength(500);
            });

            // Configure Government
            modelBuilder.Entity<Government>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                
                entity.HasMany(e => e.Cities)
                    .WithOne(c => c.Government)
                    .HasForeignKey(c => c.GovernmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure City
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });

            // Configure Address
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Details).IsRequired().HasMaxLength(500);

                entity.HasOne(e => e.Government)
                    .WithMany()
                    .HasForeignKey(e => e.GovernmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.City)
                    .WithMany()
                    .HasForeignKey(e => e.CityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ApplicationUser)
                    .WithOne(u => u.AddressNavigation)
                    .HasForeignKey<Address>(e => e.ApplicationUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ContactUs
            modelBuilder.Entity<ContactUs>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.UserEmail).HasMaxLength(200);
                entity.Property(e => e.UserPhone).HasMaxLength(50);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.AdminResponse).HasMaxLength(2000);
                entity.Property(e => e.SubmittedAt).IsRequired();
                entity.Property(e => e.IsRead).IsRequired();
            });

            // Configure Order
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SellerId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.NumberOfDays).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasOne(e => e.Seller)
                    .WithMany()
                    .HasForeignKey(e => e.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Address)
                    .WithMany()
                    .HasForeignKey(e => e.AddressId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Images)
                    .WithOne(i => i.Order)
                    .HasForeignKey(i => i.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrderImage
            modelBuilder.Entity<OrderImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImagePath).IsRequired().HasMaxLength(500);
            });

            // Configure Notification
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RecipientId).IsRequired().HasMaxLength(450);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Message).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        public ArgoreuseContext()
        {
            
        }
    }
   
}
