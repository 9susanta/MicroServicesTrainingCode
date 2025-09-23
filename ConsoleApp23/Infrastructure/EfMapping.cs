using ConsoleApp23;
using CustomerManagementMicroService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerManagementMicroService.Infrastructure
{
    public class CustomerDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Program.connectionstringDB);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var owner = modelBuilder.Entity<Customer>();
            owner.OwnsOne(c => c.money, m =>
            {
                m.Property(p => p.Value)
                 .HasColumnName("Amount")
                 .HasPrecision(18, 2);                   // decimal(18,2)

                m.WithOwner();
            });
            owner.ToTable("tblCustomer");
            modelBuilder.Entity<Order>().ToTable("tblorders");
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
    }

    public class EventDbContext : DbContext
    {

        //public EventDbContext(DbContextOptions options)
        // : base(options)
        //{

        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Program.connectionstringAudit);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventRecord>().ToTable("tblEvent");
        }
        public DbSet<EventRecord> EventRecords { get; set; }
    }
}
