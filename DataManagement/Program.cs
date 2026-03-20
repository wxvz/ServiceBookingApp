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
            Data();
        }
        static void Data()
        {
            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                //Data seeding for testing purposes
                Business newBus = new Business
                {
                    Name = "Example BUsiness",
                    City = "Dublin",
                    Email = "howdy@gmail.com",
                    PhoneNumber = "+353 1 234 5678",
                    Password = "example1"
                };

                // Add to database
                db.Businesses.Add(newBus);

                    // Save changes
                db.SaveChanges();

                Console.WriteLine("=== Sample Data Added Successfully! ===");
                Console.ReadKey();

            }
        }
    }
}