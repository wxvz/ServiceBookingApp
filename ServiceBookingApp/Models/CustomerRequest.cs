using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBookingApp.Models
{
    public enum RequestType
    {
        Refund,
        Rebooking,
        Cancellation
    }

    public class CustomerRequest
    {
        public int Id { get; set; }
        public int BusinessId { get; set; }
        public int BookingId { get; set; }
        public int CustomerId { get; set; }

        public DateTime BookingDateTime { get; set; }
        public string CustomerName { get; set; }
        public RequestType Request { get; set; }

        // Navigation properties
        public virtual Business Business { get; set; }
        public virtual Booking Booking { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
