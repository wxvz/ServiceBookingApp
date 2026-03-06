using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBookingApp
{
    public class Service
    {
       

        // Primary key
        public int ServiceId { get; set; }

        // Foreign key
        public int BusinessId { get; set; }

        // Service details
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public string Description { get; set; } = string.Empty;
        


        // Navigation property
        public virtual Business Business { get; set; }
        public virtual List<Booking> Bookings { get; set; }
        
    }
}
