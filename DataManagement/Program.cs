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
                Console.WriteLine("Businesses Saved.");
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
                Console.WriteLine("Customers Saved.");

                var services = new List<Service>
                {
                    // Services for Dublin Auto Repair
                    new Service { BusinessId = businesses[0].BusinessId, Name = "Oil Change", Description = "Full synthetic oil change", Price = 80m, Duration = TimeSpan.FromMinutes(45) },
                    new Service { BusinessId = businesses[0].BusinessId, Name = "NCT Practice Test", Description = "Will preformance test you car for an upcoming NCT Test", Price = 120m, Duration = TimeSpan.FromMinutes(65) },
                    new Service { BusinessId = businesses[0].BusinessId, Name = "Brake Inspection", Description = "Full brake system check and report", Price = 50m, Duration = TimeSpan.FromMinutes(45) },
                    new Service { BusinessId = businesses[0].BusinessId, Name = "Full Car Valet", Description = "Interior and exterior deep cleaning", Price = 100m, Duration = TimeSpan.FromHours(2) },
                    new Service { BusinessId = businesses[0].BusinessId, Name = "Tire Rotation", Description = "Rotate all 4 tires", Price = 40m, Duration = TimeSpan.FromMinutes(30) },
                    new Service { BusinessId = businesses[0].BusinessId, Name = "Engine Diagnostics", Description = "Computer diagnostics to find engine faults", Price = 65m, Duration = TimeSpan.FromMinutes(45) },
                    new Service { BusinessId = businesses[0].BusinessId, Name = "Battery Replacement", Description = "Supply and fit new car battery", Price = 110m, Duration = TimeSpan.FromMinutes(30) },

                    // Services for Cork IT Support
                    new Service { BusinessId = businesses[1].BusinessId, Name = "Virus Removal", Description = "Deep scan and malware removal", Price = 100m, Duration = TimeSpan.FromHours(2) },
                    new Service { BusinessId = businesses[1].BusinessId, Name = "Screen Repair", Description = "Repair or replace screen", Price = 120m, Duration = TimeSpan.FromHours(1.5) },
                    new Service { BusinessId = businesses[1].BusinessId, Name = "OS Install", Description = "Install Windows/Linux", Price = 120m, Duration = TimeSpan.FromHours(1.5) },
                    new Service { BusinessId = businesses[1].BusinessId, Name = "Data Recovery", Description = "Recover files from damaged hard drives", Price = 150m, Duration = TimeSpan.FromHours(3) },
                    new Service { BusinessId = businesses[1].BusinessId, Name = "Network Setup", Description = "Home or small office network configuration", Price = 80m, Duration = TimeSpan.FromHours(1) },
                    new Service { BusinessId = businesses[1].BusinessId, Name = "Hardware Upgrade", Description = "Install RAM, SSD, or new GPU", Price = 60m, Duration = TimeSpan.FromHours(1) },
                    new Service { BusinessId = businesses[1].BusinessId, Name = "Cloud Backup Setup", Description = "Configure automated cloud backups", Price = 75m, Duration = TimeSpan.FromHours(1.5) },

                    // Services for Galway Barbers
                    new Service { BusinessId = businesses[2].BusinessId, Name = "Men's Haircut", Description = "Standard style haircut", Price = 25m, Duration = TimeSpan.FromMinutes(30) },
                    new Service { BusinessId = businesses[2].BusinessId, Name = "Beard Trim", Description = "Professional beard grooming", Price = 15m, Duration = TimeSpan.FromMinutes(20) },
                    new Service { BusinessId = businesses[2].BusinessId, Name = "Hot Towel Shave", Description = "Traditional straight razor shave", Price = 30m, Duration = TimeSpan.FromMinutes(30) },
                    new Service { BusinessId = businesses[2].BusinessId, Name = "Kids Haircut", Description = "Standard haircut for children under 12", Price = 15m, Duration = TimeSpan.FromMinutes(20) },
                    new Service { BusinessId = businesses[2].BusinessId, Name = "Head Shave", Description = "Full clean head shave with razor", Price = 20m, Duration = TimeSpan.FromMinutes(30) },
                    new Service { BusinessId = businesses[2].BusinessId, Name = "Hair & Beard Combo", Description = "Full grooming package", Price = 45m, Duration = TimeSpan.FromMinutes(50) },

                    // Services for Limerick Cleaning
                    new Service { BusinessId = businesses[3].BusinessId, Name = "House Cleaning", Description = "3 hours standard cleaning", Price = 150m, Duration = TimeSpan.FromHours(3) },
                    new Service { BusinessId = businesses[3].BusinessId, Name = "Carpet Deep Clean", Description = "Steam cleaning for up to 3 rooms", Price = 120m, Duration = TimeSpan.FromHours(2) },
                    new Service { BusinessId = businesses[3].BusinessId, Name = "Window Washing", Description = "Exterior and interior window cleaning", Price = 80m, Duration = TimeSpan.FromHours(1.5) },
                    new Service { BusinessId = businesses[3].BusinessId, Name = "Deep Cleaning", Description = "Intense deep clean for tenancy turnover", Price = 250m, Duration = TimeSpan.FromHours(5) },
                    new Service { BusinessId = businesses[3].BusinessId, Name = "Oven Deep Cleaning", Description = "Professional grease removal", Price = 60m, Duration = TimeSpan.FromHours(1.5) },

                    // Services for Waterford Plumbers
                    new Service { BusinessId = businesses[4].BusinessId, Name = "Pipe Repair", Description = "Fix leaking pipes", Price = 90m, Duration = TimeSpan.FromHours(1) },
                    new Service { BusinessId = businesses[4].BusinessId, Name = "Drain Unblocking", Description = "Clear blocked sinks or main drains", Price = 75m, Duration = TimeSpan.FromHours(1) },
                    new Service { BusinessId = businesses[4].BusinessId, Name = "Boiler Servicing", Description = "Annual gas or oil boiler maintenance", Price = 110m, Duration = TimeSpan.FromHours(1.5) },
                    new Service { BusinessId = businesses[4].BusinessId, Name = "Radiator Installation", Description = "Install a new radiator unit", Price = 130m, Duration = TimeSpan.FromHours(2) },
                    new Service { BusinessId = businesses[4].BusinessId, Name = "Emergency Callout", Description = "Emergency leak or burst pipe response", Price = 200m, Duration = TimeSpan.FromHours(1) }
                };
                db.Services.AddRange(services);
                db.SaveChanges();
                Console.WriteLine("Services Saved..");

                // Add Service Schedules
                Console.WriteLine("Generating Service Schedules...");
                var schedules = new List<ServiceSchedule>();
                foreach (var service in services)
                {
                    // For each service, create a schedule for Monday to Friday, 9:00 AM to 5:00 PM
                    for (int day = 1; day <= 5; day++) 
                    {
                        schedules.Add(new ServiceSchedule
                        {
                            ServiceId = service.ServiceId,
                            DayOfWeek = (DayOfWeek)day,
                            StartTime = new TimeSpan(9, 0, 0), // 09:00 AM
                            EndTime = new TimeSpan(17, 0, 0),  // 05:00 PM
                            IsActive = true
                        });
                    }
                    
                    // Saturday schedule for half day (9:00 AM to 1:00 PM) for variety
                    schedules.Add(new ServiceSchedule
                    {
                        ServiceId = service.ServiceId,
                        DayOfWeek = DayOfWeek.Saturday,
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(13, 0, 0),
                        IsActive = true
                    });
                }
                db.ServiceSchedules.AddRange(schedules);
                db.SaveChanges();
                Console.WriteLine("Complete..");

                var random = new Random();
                var bookings = new List<Booking>();
                Console.WriteLine("Generating Bookings...");
                for (int i = 0; i < 60; i++) // Generate 50 random bookings over different months
                {
                    var customer = customers[random.Next(customers.Count)];
                    var service = services[random.Next(services.Count)];
                    var business = businesses.First(b => b.BusinessId == service.BusinessId);
                    // Get the schedule for the selected service
                    var serviceSchedule = schedules.First(s => s.ServiceId == service.ServiceId);
                    // Ensure the booking date falls on a day when the service is available
                    var availableDays = schedules
                        .Where(s => s.ServiceId == service.ServiceId && s.IsActive)
                        .Select(s => s.DayOfWeek)
                        .ToList();
                    // Randomly select a day of the week from the available schedule
                    var randomDayOfWeek = availableDays[random.Next(availableDays.Count)];
                    // Get the available time slots for that day
                    var availableTimes = schedules
                        .Where(s => s.ServiceId == service.ServiceId && s.DayOfWeek == randomDayOfWeek)
                        .Select(s => new { s.StartTime, s.EndTime })
                        .ToList();
                    // Randomly select a time slot from the available times
                    var randomTimeSlot = availableTimes[random.Next(availableTimes.Count) ];
                    // Random time within the selected time slot (int)eger minutes between start and end time
                    // Ensure we have enough time before EndTime to actually complete the service
                    int availableMinutes = (int)(randomTimeSlot.EndTime - randomTimeSlot.StartTime).TotalMinutes;
                    int maxStartOffsetMinutes = availableMinutes - (int)service.Duration.TotalMinutes;

                    // Fallback to 0 if maxStartOffsetMinutes is negative (if duration is somehow longer than the slot)
                    maxStartOffsetMinutes = Math.Max(0, maxStartOffsetMinutes);

                    // Random time within the selected time slot, ensuring it finishes before EndTime
                    var bookingTime = randomTimeSlot.StartTime.Add(TimeSpan.FromMinutes(random.Next(0, maxStartOffsetMinutes)));

                    // Random date within the last 3 months to next 1 month
                    var randomDays = random.Next(-90, 30);
                    var bookingDate = DateTime.Now.AddDays(randomDays);

                    // Ensure the booking date falls on the correct day of the week for the service schedule
                    int dayOffset = randomDayOfWeek - bookingDate.DayOfWeek; 
                    // if bookingDate is a Tuesday and randomDayOfWeek is Thursday, offset will be 2 so we need to add 2 days to get to the correct day of the week
                    bookingDate = bookingDate.AddDays(dayOffset); // Adjust the booking date to the correct day of the week

                    var booking = new Booking
                    {
                        CustomerId = customer.CustomerId,
                        BusinessId = business.BusinessId,
                        ServiceId = service.ServiceId,
                        Date = bookingDate,
                        Time = bookingTime, // Random time within the selected slot
                        Status = bookingDate < DateTime.Now ? BookingStatus.Completed : BookingStatus.Pending // Completed if in the past, otherwise pending
                    };

                    // Attaching the Payment to the booking navigation property
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
                    Console.WriteLine("Complete..");
                }

                db.Bookings.AddRange(bookings);
                db.SaveChanges();

                Console.WriteLine("=== Sample Data Added Successfully! ===");
                Console.WriteLine($"Added: {businesses.Count} Businesses, {customers.Count} Customers, {services.Count} Services, {schedules.Count} Schedules, {bookings.Count} Bookings.");
                Console.ReadKey();
            }
        }
    }
}