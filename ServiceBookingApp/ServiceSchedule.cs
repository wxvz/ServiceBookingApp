using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceBookingApp
{
    public class ServiceSchedule
    {
        // Primary key
        public int ServiceScheduleId { get; set; }


        // Schedule details
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; } = true;  // To disable specific days

        // Navigation property
        public virtual Service Service { get; set; }
    }
}