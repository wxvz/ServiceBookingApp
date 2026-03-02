using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBookingApp
{
    internal class BookingSchedule
    {

        // Primary key
        public int Id { get; set; }
        // Foreign key
        public int ServiceId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // Navigation property
        public virtual Service Service { get; set; }
    }
}
