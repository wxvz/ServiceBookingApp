namespace ServiceBookingApp.Migrations
{
    using ServiceBookingApp;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Windows.Controls;

    internal sealed class Configuration : DbMigrationsConfiguration<ServiceBookingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ServiceBookingApp.ServiceBookingContext";
        }

        protected override void Seed(ServiceBookingContext context)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("1234");

            //Seeeeeeeding Dataaaaaa
            context.Businesses.AddOrUpdate(
                b => b.Email,
                new Business
                {
                    Name = "Dublin Hair & Beauty",
                    City = "Dublin",
                    Email = "admin@example.ie",
                    PhoneNumber = "+353 1 234 5678",
                    Password = hashedPassword 
                }
            );

            // Save changes so the Business Id is generated for dependents
            context.SaveChanges(); 

            var adminBusiness = context.Businesses.FirstOrDefault(b => b.Email == "admin@example.ie");

            context.Customers.AddOrUpdate(
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

            context.Services.AddOrUpdate(
                s => s.Name,
                new Service
                {
                    BusinessId = adminBusiness.BusinessId,
                    Name = "Haircut & Style",
                    Price = 45.00m,
                    Duration = TimeSpan.FromMinutes(45),
                    Description = "Professional haircut and styling service"
                }
            );

            context.SaveChanges();

            var haircut = context.Services.FirstOrDefault(s => s.Name == "Haircut & Style");
            var guestCustomer = context.Customers.FirstOrDefault(c => c.Email == "guest@example.ie");

            context.Bookings.AddOrUpdate(
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
            context.SaveChanges();
            var Booking = context.Bookings.FirstOrDefault(b => b.CustomerId == guestCustomer.CustomerId && b.ServiceId == haircut.ServiceId);

            context.Payments.AddOrUpdate(
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

            context.SaveChanges();
        }
    }
}
