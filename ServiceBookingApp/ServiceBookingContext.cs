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
    }
}
