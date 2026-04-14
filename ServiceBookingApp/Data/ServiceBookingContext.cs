using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceBookingApp.Models;

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
        public DbSet<CustomerRequest> CustomerRequests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // One to one relationship between Booking and Payment
            modelBuilder.Entity<Booking>()
                .HasOptional(b => b.Payment)
                .WithRequired(p => p.Booking);
                
            // Service (Principal) to ServiceSchedule (Dependent)
            modelBuilder.Entity<Service>()
                .HasMany(s => s.ServiceSchedules)
                .WithRequired(ss => ss.Service)
                .HasForeignKey(ss => ss.ServiceId)
                .WillCascadeOnDelete(true); // Cascade delete schedules when a service is deleted

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

            // Customer -> Bookings: Prevent cascade delete on Customer deletion##
            modelBuilder.Entity<Booking>()
                .HasRequired(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .WillCascadeOnDelete(false);
            // Customer -> CustomerRequests: Prevent cascade delete on Customer deletion
            modelBuilder.Entity<CustomerRequest>()
                .HasRequired(cr => cr.Customer)
                .WithMany(c => c.CustomerRequests)
                .HasForeignKey(cr => cr.CustomerId)
                .WillCascadeOnDelete(false);


            // Business -> CustomerRequests: Prevent cascade delete on Business deletion
            modelBuilder.Entity<CustomerRequest>()
                .HasRequired(cr => cr.Business)
                .WithMany(b => b.CustomerRequests)
                .HasForeignKey(cr => cr.BusinessId)
                .WillCascadeOnDelete(false);

            // Customer -> CustomerRequests: Prevent cascade delete on Customer deletion
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.CustomerRequests)
                .WithRequired(cr => cr.Customer)
                .HasForeignKey(cr => cr.CustomerId)
                .WillCascadeOnDelete(false);

            // Booking -> CustomerRequests: Prevent cascade delete on Booking deletion
            modelBuilder.Entity<CustomerRequest>()
                .HasRequired(cr => cr.Booking)
                .WithMany() 
                .HasForeignKey(cr => cr.BookingId)
                .WillCascadeOnDelete(false);

        }
    }
}
