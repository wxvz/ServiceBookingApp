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
            
        }
        static void Data()
        {
            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                //Data seeding for testing purposes

                // Add to database
                //db.Businesses.Add();
               
                // Save changes
                db.SaveChanges();

                Console.WriteLine("=== Sample Data Added Successfully! ===");
                Console.ReadKey();

            }
        }
    }
}