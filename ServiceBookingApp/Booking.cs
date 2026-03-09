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
        public int CustomerId { get; set; }
        public int BusinessId { get; set; } 
        public int ServiceId { get; set; } 

        // Booking details
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public BookingStatus BookingStatus { get; set; }

        // Navigation properties
        public virtual Business Business { get; set; }
        public virtual Service Service { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual Customer Customer { get; set; }
 
    }
    
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled
    }
    
}
