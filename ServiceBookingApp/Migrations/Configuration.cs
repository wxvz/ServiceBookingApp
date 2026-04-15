namespace ServiceBookingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ServiceBookingApp.Models;
    using ServiceBookingApp.Data;

    internal sealed class Configuration : DbMigrationsConfiguration<ServiceBookingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ServiceBookingApp.ServiceBookingContext";
        }

        protected override void Seed(ServiceBookingContext db)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("1234");

            //Seeeeeeeding Dataaaaaa
            db.Businesses.AddOrUpdate(
                b => b.Email,
                new Business
                {
                    Name = "Dublin Hair & Beauty",
                    Address = "11, O'Connell Street, Dublin",
                    Email = "admin@example.ie",
                    PhoneNumber = "+353 1 234 5678",
                    Password = hashedPassword 
                }
            );

            // Save changes so the Business Id is generated for dependents
            db.SaveChanges(); 

            var adminBusiness = db.Businesses.FirstOrDefault(b => b.Email == "admin@example.ie");

            db.Customers.AddOrUpdate(
                c => c.Email,
                new Customer
                {
                    Name = "Guest",
                    Email = "guest@example.ie",
                    Address = "15 O'Connell Street, Dublin 1",
                    PhoneNumber = "+353 87 123 4567",
                    Password = "guest123"
                }
            );

            db.Services.AddOrUpdate(
                s => s.Name,
                new Service
                {
                    BusinessId = adminBusiness.BusinessId,
                    Name = "Haircut & Style",
                    Price = 45.00m,
                    Duration = TimeSpan.FromMinutes(45),
                    Description = "Professional haircut and styling service",
                    IsActive = false
                }
            );

            db.SaveChanges();

            var haircut = db.Services.FirstOrDefault(s => s.Name == "Haircut & Style");
            var guestCustomer = db.Customers.FirstOrDefault(c => c.Email == "guest@example.ie");

            db.Bookings.AddOrUpdate(
                b => new { b.CustomerId, b.ServiceId },
                new Booking
                {
                    CustomerId = guestCustomer.CustomerId,
                    BusinessId = adminBusiness.BusinessId,
                    ServiceId = haircut.ServiceId,
                    Date = DateTime.Now.AddDays(1),
                    Time = new TimeSpan(10, 0, 0),
                    Status = BookingStatus.Confirmed
                }
            );
            db.SaveChanges();
            var Booking = db.Bookings.FirstOrDefault(b => b.CustomerId == guestCustomer.CustomerId && b.ServiceId == haircut.ServiceId);

            db.Payments.AddOrUpdate(
                p => p.PaymentId, // The primary key is also the foreign key
                new Payment
                {
                    PaymentId = Booking.BookingId, // Link directly to the bookings ID
                    BusinessId = adminBusiness.BusinessId, // Required by Payment class
                    Amount = haircut.Price,
                    PaymentDate = DateTime.Now,
                    Method = PaymentMethod.Card, 
                    Status = PaymentStatus.Completed 
                }
            );

            db.SaveChanges();
        }
    }
}
