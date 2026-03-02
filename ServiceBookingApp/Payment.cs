using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBookingApp
{
    public class Payment
    {
        
        // Primary key
        public int PaymentId { get; set; }

        // Foreign keys
        public int BookingId { get; set; }

        // Payment details
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public Enum PaymentStatus { get; set; }
        public Enum PaymentMethod { get; set; }

        // Navigation properties
        public Booking Booking { get; set; }
        public Business Business { get; set; }

    }
    // Enums for payment status and method
    enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }
    enum PaymentMethod
    {
        Card,
        Cash
    }
}
