using Microsoft.EntityFrameworkCore;
using NetCoreUnitTestProject.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreUnitTestProject.Data.DbContexts
{
    public class UnitTestDbContext : DbContext
    {
        public UnitTestDbContext(DbContextOptions<UnitTestDbContext> options):base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
