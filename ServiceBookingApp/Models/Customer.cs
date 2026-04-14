using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBookingApp.Models
{
    public class Customer
    {
        // Primary key
        public int CustomerId { get; set; }

        // Customer details
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; }  = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber {  get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;

        // Navigation properties
        public virtual List<Booking> Bookings { get; set; }
        public virtual List<CustomerRequest> CustomerRequests { get; set; }

    }
}
