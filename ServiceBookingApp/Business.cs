using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBookingApp
{
    public class Business
    {

        // Primary key
        public int BusinessId { get; set; }

        // Business details
        public string Name { get; set; } = string.Empty;
        public string City{ get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Navigation properties
        public virtual List<Service> Services { get; set; }
        public virtual List<Booking> Bookings { get; set; }

    }
    
}
