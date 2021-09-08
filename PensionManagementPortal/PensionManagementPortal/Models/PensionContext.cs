using System;
using Microsoft.EntityFrameworkCore;

namespace PensionManagementPortal.Models
{
    public class PensionContext : DbContext
    {
        public PensionContext(DbContextOptions<PensionContext> options):base(options) { }

        public DbSet<ProcessedPensionDetail> ProcessedPensionDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessedPensionDetail>().ToTable("PensionDetails");
        }
    }
}
