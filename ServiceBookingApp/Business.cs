using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBookingApp
{
    public class Business
    {
        public string Name { get; set; } = string.Empty;
        public string City{ get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public List<Service> Services { get; set; }
        public List<Booking> Bookings { get; set; }

    }
    
}
