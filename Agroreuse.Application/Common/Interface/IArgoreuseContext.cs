using Agroreuse.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Agroreuse.Application.Common.Interface
{
    public interface IArgoreuseContext 
    {
        DbSet<ApplicationUser> Users { get; set; }
    }
}
