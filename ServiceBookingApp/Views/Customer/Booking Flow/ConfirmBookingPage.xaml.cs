using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ServiceBookingApp.Data;
using ServiceBookingApp.Helper;
using ServiceBookingApp.Models;


namespace ServiceBookingApp.Views.Customer.Booking_Flow
{
    /// <summary>
    /// Interaction logic for ConfirmBookingPage.xaml
    /// </summary>
    public partial class ConfirmBookingPage : Page
    {
        private readonly int _serviceId;
        private ServiceBookingContext db = new ServiceBookingContext();
        private Service _selectedService;
        private List<ServiceSchedule> _serviceSchedules;
        private List<Booking> _existingBookings;

        public ConfirmBookingPage(int serviceId)
        {
            InitializeComponent();
            _serviceId = serviceId;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadServiceDetails();
        }

        private void LoadServiceDetails()
        {
            try
            {
                _selectedService = db.Services.FirstOrDefault(s => s.ServiceId == _serviceId);
                
                if (_selectedService != null)
                {
                    ServiceNameText.Text = _selectedService.Name;
                    ServiceDetailsText.Text = $"Price: {_selectedService.Price:C} | Duration: {_selectedService.Duration:hh\\:mm}";

                    // Load schedules for this service
                    _serviceSchedules = db.ServiceSchedules.Where(ss => ss.ServiceId == _serviceId && ss.IsActive).ToList();
                    
                    // Set DatePicker minimum date
                    BookingDatePicker.DisplayDateStart = DateTime.Today;
                }
                else
                {
                    MessageBox.Show("Service not found.");
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading service: {ex.Message}");
            }
        }

        private void BookingDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BookingDatePicker.SelectedDate.HasValue)
            {
                LoadAvailableTimeSlots(BookingDatePicker.SelectedDate.Value);
            }
        }

        private void LoadAvailableTimeSlots(DateTime selectedDate)
        {
            try
            {
                AvailableTimeComboBox.ItemsSource = null;
                AvailableTimeComboBox.Items.Clear();

                // Get day of week for selected date
                var dayOfWeek = selectedDate.DayOfWeek;

                // Find schedule for this day
                var schedule = _serviceSchedules.FirstOrDefault(ss => ss.DayOfWeek == dayOfWeek);

                // If no schedule exists for this day or if its today and past the end time, show no slots available
                if (schedule == null || (selectedDate.Date == DateTime.Today && DateTime.Now.TimeOfDay >= schedule.EndTime))
                {
                    ShowNoSlotsAvailable();
                    return;
                }

                // Load existing bookings for this specific date and service to check for conflicts
                _existingBookings = db.Bookings
                    .Where(b => b.ServiceId == _serviceId && b.Date == selectedDate.Date && 
                               (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed))
                    .ToList();

                var availableSlots = new List<TimeSpan>();
                var slotDuration = _selectedService.Duration;
                
                // Calculate start time. If today, start from now (rounded to next 30 min slot) or schedule start
                TimeSpan bookingSlot = schedule.StartTime;
                if (selectedDate.Date == DateTime.Today)
                {
                    var now = DateTime.Now.TimeOfDay;
                    if (now > bookingSlot)
                    {
                        // Rpound up to the next 30 minute mark
                        bookingSlot = new TimeSpan(now.Hours + (now.Minutes >= 30 ? 1 : 0), now.Minutes < 30 ? 30 : 0, 0);
                    }
                }

                // Generate slots based on service duration
                while (bookingSlot.Add(slotDuration) <= schedule.EndTime)
                {
                    // Check if this slot conflicts with any existing bookings
                    bool isConflict = _existingBookings
                        .Any(b => (bookingSlot >= b.Time && bookingSlot < b.Time.Add(_selectedService.Duration)) || // New slot starts during an existing booking
                        (bookingSlot.Add(slotDuration) > b.Time && bookingSlot.Add(slotDuration) <= b.Time.Add(_selectedService.Duration)) || // New slot ends during an existing booking
                        (bookingSlot <= b.Time && bookingSlot.Add(slotDuration) >= b.Time.Add(_selectedService.Duration)) // New slot completely overlaps an existing booking
                    );

                    if (!isConflict) // If no conflict add to available slots
                    {
                        availableSlots.Add(bookingSlot);
                    }
                    
                    // Move to next slot
                    bookingSlot = bookingSlot.Add(slotDuration);
                }

                if (availableSlots.Count > 0)
                {
                    AvailableTimeComboBox.ItemsSource = availableSlots.Select(t => t.ToString(@"hh\:mm")).ToList();
                    AvailableTimeComboBox.Visibility = Visibility.Visible;
                    AvailableTimeComboBox.SelectedIndex = 0;
                    NoSlotsText.Visibility = Visibility.Collapsed;
                    ConfirmButton.IsEnabled = true;
                }
                else
                {
                    ShowNoSlotsAvailable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating slots: {ex.Message}");
            }
        }

        private void ShowNoSlotsAvailable()
        {
            AvailableTimeComboBox.Visibility = Visibility.Collapsed;
            NoSlotsText.Visibility = Visibility.Visible;
            ConfirmButton.IsEnabled = false;
        }

        private void ConfirmBooking_Click(object sender, RoutedEventArgs e)
        {
            if (!BookingDatePicker.SelectedDate.HasValue || AvailableTimeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a date and time slot.");
                return;
            }

            if (SessionManager.CurrentCustomer == null)
            {
                MessageBox.Show("You must be logged in to book a service.");
                return;
            }

            try
            {
                DateTime bookingDate = BookingDatePicker.SelectedDate.Value.Date;
                TimeSpan bookingTime = TimeSpan.Parse(AvailableTimeComboBox.SelectedItem.ToString());

                // Final conflict check before saving
                bool conflict = db.Bookings.Any(b => 
                    b.ServiceId == _serviceId && 
                    b.Date == bookingDate && 
                    b.Time == bookingTime && 
                    (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending));

                if (conflict)
                {
                    MessageBox.Show("Sorry, this time slot has just been booked. Please select another time.");
                    LoadAvailableTimeSlots(bookingDate); // Refresh slots
                    return;
                }

                var newBooking = new Booking
                {
                    CustomerId = SessionManager.CurrentCustomer.CustomerId,
                    BusinessId = _selectedService.BusinessId,
                    ServiceId = _selectedService.ServiceId,
                    Date = bookingDate,
                    Time = bookingTime,
                    Status = BookingStatus.Pending
                };

                db.Bookings.Add(newBooking);
                db.SaveChanges();

                MessageBox.Show("Booking confirmed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Navigate back to the main search page or the customer bookings page
                NavigationService.Navigate(new BookServicePage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error confirming booking: {ex.Message}");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}





