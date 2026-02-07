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
