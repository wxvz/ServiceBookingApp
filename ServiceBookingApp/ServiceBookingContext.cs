using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBookingApp
{
    public class ServiceBookingContext : DbContext
    {
        public ServiceBookingContext() : base("ServiceBookingData") {}
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<ServiceSchedule> ServiceSchedules { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Booking (Principal) to Payment (Dependent)
            modelBuilder.Entity<Booking>()
                .HasOptional(b => b.Payment)
                .WithRequired(p => p.Booking);

            // Service (Principal) to ServiceSchedule (Dependent)
            modelBuilder.Entity<Service>()
                .HasOptional(s => s.ServiceSchedule)
                .WithRequired(ss => ss.Service);

            // Bookings in a business with many bookings and have business id will not cascade delete
            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.Business)
                .WithMany(bus => bus.Bookings)
                .HasForeignKey(b => b.BusinessId)
                .WillCascadeOnDelete(false);

            // Business -> Bookings: Prevent cascade delete on Business deletion
            modelBuilder.Entity<Payment>()
                .HasRequired(p => p.Business)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BusinessId)
                .WillCascadeOnDelete(false);

            // Business -> Bookings: Prevent cascade delete on Business deletion
            modelBuilder.Entity<Business>()
                .HasMany(b => b.Bookings)
                .WithRequired(b => b.Business)
                .HasForeignKey(b => b.BusinessId)
                .WillCascadeOnDelete(false);
        }
    }
}
