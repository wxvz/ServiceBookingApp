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
            //Run to populate the database with dummy data for testing
            //Data();
        }

        static void Data()
        {
            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                Console.WriteLine("Generating dummy data...");
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
                var businesses = new List<Business>
                {
                    new Business { Name = "Dublin Auto Repair", Address = "Example Street, Dublin", Email = "contact@dublinauto.ie", PhoneNumber = "01 234 5678", Password = hashedPassword },
                    new Business { Name = "Cork IT Support", Address = "Example Street, Cork", Email = "support@corkit.ie", PhoneNumber = "021 987 6543", Password = hashedPassword },
                    new Business { Name = "Galway Barbers", Address = "Example Street, Galway", Email = "info@galwaybarbers.ie", PhoneNumber = "091 123 4567", Password = hashedPassword },
                    new Business { Name = "Limerick Cleaning", Address = "Example Street, Limerick", Email = "cleaningsouth@limerick.ie", PhoneNumber = "061 456 7890", Password = hashedPassword },
                    new Business { Name = "Waterford Plumbers", Address = "Example Street, Waterford", Email = "plumber@waterford.ie", PhoneNumber = "083 654 3210", Password = hashedPassword }
                };
                db.Businesses.AddRange(businesses);
                db.SaveChanges(); // Save to generate IDs

                var customers = new List<Customer>
                {
                    new Customer { Name = "John Doe", Email = "john.d@example.com", PhoneNumber = "087 111 2222", Password = hashedPassword },
                    new Customer { Name = "Jane Smith", Email = "jane.s@example.com", PhoneNumber = "086 333 4444", Password = hashedPassword },
                    new Customer { Name = "Alice Johnson", Email = "alice.j@example.com", PhoneNumber = "085 555 6666", Password = hashedPassword },
                    new Customer { Name = "Bob Brown", Email = "bob.b@example.com", PhoneNumber = "089 777 8888", Password = hashedPassword },
                    new Customer { Name = "Charlie Davis", Email = "charlie.d@example.com", PhoneNumber = "087 999 0000", Password = hashedPassword },
                    new Customer { Name = "Diana Evans", Email = "diana.e@example.com", PhoneNumber = "086 222 3333", Password = hashedPassword },
                    new Customer { Name = "Evan Foster", Email = "evan.f@example.com", PhoneNumber = "085 444 5555", Password = hashedPassword },
                    new Customer { Name = "Fiona Green", Email = "fiona.g@example.com", PhoneNumber = "083 666 7777", Password = hashedPassword }
                };
                db.Customers.AddRange(customers);
                db.SaveChanges();

                var services = new List<Service>
                {
                    new Service { BusinessId = businesses[0].BusinessId, Name = "Oil Change", Description = "Full synthetic oil change", Price = 80m, Duration = TimeSpan.FromMinutes(45) },
                    new Service { BusinessId = businesses[0].BusinessId, Name = "Tire Rotation", Description = "Rotate all 4 tires", Price = 40m, Duration = TimeSpan.FromMinutes(30) },
                    new Service { BusinessId = businesses[1].BusinessId, Name = "Virus Removal", Description = "Deep scan and malware removal", Price = 100m, Duration = TimeSpan.FromHours(2) },
                    new Service { BusinessId = businesses[1].BusinessId, Name = "OS Install", Description = "Install Windows/Linux", Price = 120m, Duration = TimeSpan.FromHours(1.5) },
                    new Service { BusinessId = businesses[2].BusinessId, Name = "Men's Haircut", Description = "Standard style haircut", Price = 25m, Duration = TimeSpan.FromMinutes(30) },
                    new Service { BusinessId = businesses[2].BusinessId, Name = "Beard Trim", Description = "Professional beard grooming", Price = 15m, Duration = TimeSpan.FromMinutes(20) },
                    new Service { BusinessId = businesses[3].BusinessId, Name = "House Cleaning", Description = "3 hours standard cleaning", Price = 150m, Duration = TimeSpan.FromHours(3) },
                    new Service { BusinessId = businesses[4].BusinessId, Name = "Pipe Repair", Description = "Fix leaking pipes", Price = 90m, Duration = TimeSpan.FromHours(1) }
                };
                db.Services.AddRange(services);
                db.SaveChanges();

                var random = new Random();
                var bookings = new List<Booking>();

                for (int i = 0; i < 30; i++) // Generate 30 random bookings over different months
                {
                    var customer = customers[random.Next(customers.Count)];
                    var service = services[random.Next(services.Count)];
                    var business = businesses.First(b => b.BusinessId == service.BusinessId);
                    
                    // Random date within the last 3 months to next 1 month
                    var randomDays = random.Next(-90, 30);
                    var bookingDate = DateTime.Now.AddDays(randomDays);

                    var booking = new Booking
                    {
                        CustomerId = customer.CustomerId,
                        BusinessId = business.BusinessId,
                        ServiceId = service.ServiceId,
                        Date = bookingDate.Date,
                        Time = new TimeSpan(random.Next(9, 17), 0, 0), // Random time between 9AM and 5PM
                        Status = bookingDate < DateTime.Now ? BookingStatus.Completed : BookingStatus.Pending // Completed if in the past, otherwise pending
                    };

                    // Attaching the Payment DIRECTLY to the booking navigation property
                    if (booking.Status == BookingStatus.Completed || random.NextDouble() > 0.5) // Randomly decide to create a payment for pending bookings. if number generated greater than 0.5
                    {
                        booking.Payment = new Payment
                        {
                            BusinessId = business.BusinessId,
                            Amount = service.Price,
                            PaymentDate = bookingDate,
                            Status = bookingDate < DateTime.Now ? PaymentStatus.Completed : PaymentStatus.Pending, // Completed if in the past, otherwise pending
                            Method = (PaymentMethod)random.Next(0, 3) // Cash, Card, etc. depending on enum
                        };
                    }

                    bookings.Add(booking);
                }

                db.Bookings.AddRange(bookings);
                db.SaveChanges();

                Console.WriteLine("=== Sample Data Added Successfully! ===");
                Console.WriteLine($"Added: {businesses.Count} Businesses, {customers.Count} Customers, {services.Count} Services, {bookings.Count} Bookings.");
                Console.ReadKey();
            }
        }
    }
}