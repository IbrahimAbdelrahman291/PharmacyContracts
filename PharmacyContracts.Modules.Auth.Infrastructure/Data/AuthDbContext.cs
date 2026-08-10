using Microsoft.EntityFrameworkCore;
using PharmacyContracts.Modules.Auth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyContracts.Modules.Auth.Infrastructure.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
