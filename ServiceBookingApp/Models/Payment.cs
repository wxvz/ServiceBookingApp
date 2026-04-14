using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBookingApp.Models
{
    public class Payment
    {
        
        // Primary key
        public int PaymentId { get; set; }

        // Foreign keys
        public int BusinessId { get; set; }

        // Payment details
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentMethod Method { get; set; }

        // Navigation properties
        public virtual Booking Booking { get; set; }
        public virtual Business Business { get; set; }

    }
    
    // Enums for payment status and method
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }
    
    public enum PaymentMethod
    {
        Card,
        Cash
    }
}
