using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBookingApp
{
    public class Booking
    {
      
        // Primary key
        public int BookingId { get; set; }

        // Foreign keys
        public int BusinessId { get; set; } 
        public int ServiceId { get; set; }
        public int PaymentId { get; set; }

        // Customer detail
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public Enum BookingStatus { get; set; }

        // Navigation properties
        public virtual Business Business { get; set; }
        public virtual Service Service { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual Customer Customer { get; set; }
 
    }
    enum BookingStatus
    {
        Complete,
        Pending
    }
    
}
