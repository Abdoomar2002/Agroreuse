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
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        public ArgoreuseContext()
        {
            
        }
    }
   
}
