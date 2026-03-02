using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBookingApp
{
    internal class Customer
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; }  = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber {  get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;
        public virtual List<Booking> Bookings { get; set; }

    }
}
