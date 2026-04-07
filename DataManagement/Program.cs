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
            //Data();
        }
        static void Data()
        {
            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                //Data seeding for testing purposes
                
                

                // Add to database


                // Save changes
                db.SaveChanges();

                Console.WriteLine("=== Sample Data Added Successfully! ===");
                Console.ReadKey();

            }
        }
        public void NewPayment()
        {
            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                Payment payment = new Payment
                {
                    BusinessId = 1,
                    Amount = 120m,
                    PaymentDate = DateTime.Now,
                    Status = PaymentStatus.Pending,
                    Method = PaymentMethod.Cash
                };
                db.Payments.Add(payment);
            }
        }
        public void NewBooking()
        {
            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                Booking booking = new Booking
                {
                    CustomerId = 1,
                    BusinessId = 1,
                    ServiceId = 2,
                    Date = DateTime.Now,
                    Time = TimeSpan.FromMinutes(25),
                    Status = BookingStatus.Pending
                };
                db.Bookings.Add(booking);
            }
        }
        public void NewBusiness()
        {
            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                Business newBus = new Business
                {
                    Name = "Example BUsiness",
                    City = "Dublin",
                    Email = "howdy@gmail.com",
                    PhoneNumber = "+353 1 234 5678",
                    Password = "example1"
                };
            }
        }

    }
}