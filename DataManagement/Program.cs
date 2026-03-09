using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceBookingApp;

namespace DataManagement
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                // Example Business
               /*
                var adminBusiness = new Business
                {
                    Name = "Dublin Hair & Beauty",
                    City = "Dublin",
                    Email = "admin@dublinhairbeauty.ie",
                    PhoneNumber = "+353 1 234 5678",
                    Password = "admin123" // TODO: Hash with BCrypt
                };

                // Example Customer
                var guestCustomer = new Customer
                {
                    Name = "Guest",
                    Email = "guest@example.ie",
                    Address = "15 O'Connell Street, Dublin 1",
                    PhoneNumber = "+353 87 123 4567",
                    Password = "guest123" // TODO: Hash with BCrypt
                };

                // Example Service
                var haircut = new Service
                {
                    Business = adminBusiness,
                    Name = "Haircut & Style",
                    Price = 45.00m,
                    Duration = TimeSpan.FromMinutes(45),
                    Description = "Professional haircut and styling service"
                };

                // Example ServiceSchedule (Monday 9 AM - 6 PM)
                var schedule = new ServiceSchedule
                {
                    Service = haircut,
                    DayOfWeek = DayOfWeek.Monday,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0)
                };

                // Create example Booking
                var booking = new Booking
                {
                    Customer = guestCustomer,
                    Business = adminBusiness,
                    Service = haircut,
                    Date = DateTime.Today.AddDays(7),
                    Time = new TimeSpan(10, 0, 0), // 10 AM appointment
                    BookingStatus = BookingStatus.Pending
                };

                var payment = new Payment
                {
                    Booking = booking,
                    Amount = haircut.Price,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = PaymentMethod.Card,
                    PaymentStatus = PaymentStatus.Completed
                };

                // Add to database
                db.Businesses.Add(adminBusiness);
                db.Customers.Add(guestCustomer);
                db.Services.Add(haircut);
                db.ServiceSchedules.Add(schedule);
                db.Bookings.Add(booking);

                // Save changes
                db.SaveChanges();

                Console.WriteLine("=== Sample Data Created Successfully! ===");
                Console.ReadKey();
               */
            }
        }
    }
}