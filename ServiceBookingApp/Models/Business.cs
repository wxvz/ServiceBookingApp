using System.Collections.Generic;

namespace ServiceBookingApp.Models
{
    public class Business
    {

        // Primary key
        public int BusinessId { get; set; }

        // Business details
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Navigation properties
        public virtual List<Service> Services { get; set; }
        public virtual List<Booking> Bookings { get; set; }
        public virtual List<Payment> Payments { get; set; }
        public virtual List<CustomerRequest> CustomerRequests { get; set; }

    }

}
