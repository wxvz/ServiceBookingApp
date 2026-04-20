using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ServiceBookingApp.Models;
using ServiceBookingApp.Data;
using ServiceBookingApp.Helper;

namespace ServiceBookingApp.Views.Customer.Dashboard
{
    /// <summary>
    /// Interaction logic for ManageBookingsPage.xaml
    /// </summary>
    public partial class CustomerBookingsPage : Page
    {
        public ServiceBookingContext db = new ServiceBookingContext();
        List<Booking> allBookings = new List<Booking>();
        private Booking editedBooking;

        public CustomerBookingsPage()
        {
            InitializeComponent();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBookings();
            LoadStatusFilterOptions();
        }

        private void LoadBookings()
        {
            if (SessionManager.CurrentCustomer == null)
            {
                MessageBox.Show("No customer is currently logged in.");
                return;
            }
            try
            {
                db = new ServiceBookingContext(); // refresh context
                var rawBookings = db.Bookings
                    .Where(b => b.CustomerId == SessionManager.CurrentCustomer.CustomerId)
                    .ToList();

                var today = DateTime.Today;
                bool changed = false;
                // Checks all bookings that customer has and saves any bookings that are pending or confirmed that are before today.
                foreach (var b in rawBookings)
                {
                    if ((b.Status == (BookingStatus)0 || b.Status == (BookingStatus)1) && b.Date < today) 
                    {
                        b.Status = (BookingStatus)2; // Completed
                        changed = true;
                    }
                }

                if (changed) 
                {
                    db.SaveChanges();
                }

                allBookings = rawBookings;

                FilterByBookingDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bookings: " + ex.Message);
            }
        }

        private void LoadStatusFilterOptions()
        {
            string[] status = { "-- All --", "Pending", "Confirmed", "Completed", "Cancelled" };
            BookingStatusFilterCBX.ItemsSource = status;
            BookingStatusFilterCBX.SelectedIndex = 1;
        }

        private void BookingStatusFilterCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterByBookingDetails();
        }

        private void FilterByBookingDetails()
        {
            string[] noBookingsSource = { "No bookings." };
            if (BookingStatusFilterCBX.SelectedItem == null || allBookings == null) return;
            
            if (allBookings.Count == 0)
            {
                BookingsDataGrid.ItemsSource = noBookingsSource;
                return;
            }

            string selectedStatus = BookingStatusFilterCBX.SelectedItem.ToString();
            if (selectedStatus == "-- All --")
            {
                BookingsDataGrid.ItemsSource = allBookings;
            }
            else
            {
                if (Enum.TryParse(selectedStatus, out BookingStatus parsedStatus))
                {
                    var filteredBookings = allBookings
                        .Where(b => b.Status == parsedStatus)
                        .ToList();

                    if (filteredBookings.Count == 0)
                        BookingsDataGrid.ItemsSource = noBookingsSource;
                    else
                        BookingsDataGrid.ItemsSource = filteredBookings;
                }
            }
        }

        private void EditBooking_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Booking booking)
            {
                // Verify 24 hour rule (if customer tries to edit booking within 24 hrs)
                DateTime bookingDateTime = booking.Date.Date.Add(booking.Time);
                if (bookingDateTime <= DateTime.Now.AddHours(24) && bookingDateTime > DateTime.Now)
                {
                    MessageBox.Show("Bookings can only be modified or cancelled up to 24 hours in advance of the booking time.", "Modification Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (booking.Status == BookingStatus.Completed || booking.Status == BookingStatus.Cancelled || bookingDateTime <= DateTime.Now)
                {
                    MessageBox.Show("This booking cannot be modified as it has passed, completed, or cancelled.", "Modification Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                editedBooking = booking;
                EditDatePicker.SelectedDate = booking.Date;
                EditTimeTextBox.Text = booking.Time.ToString(@"hh\:mm");
                
                BookingsDataGrid.Visibility = Visibility.Collapsed;
                EditPanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveBooking_Click(object sender, RoutedEventArgs e)
        {
            if (editedBooking == null) return;

            if (!EditDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date.");
                return;
            }

            if (!TimeSpan.TryParse(EditTimeTextBox.Text, out TimeSpan newTime))
            {
                MessageBox.Show("Please enter a valid time (HH:mm).");
                return;
            }

            DateTime newDate = EditDatePicker.SelectedDate.Value;

            // Ensures new time also respects 24hr or future rule generally. (Cant book within 24 hours for now)
            if (newDate.Date.Add(newTime) <= DateTime.Now.AddHours(24))
            {
                MessageBox.Show("New booking time must be at least 24 hours from now.", "Invalid Time", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var result = MessageBox.Show($"Are you sure you want to request a change to {editedBooking.Date.ToShortDateString()} at {editedBooking.Time}?", "Confirm Request", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var customerName = editedBooking.Customer.Name;
                    
                    var newRequest = new CustomerRequest
                    {
                        BusinessId = editedBooking.BusinessId,
                        BookingId = editedBooking.BookingId,
                        CustomerId = editedBooking.CustomerId,
                        BookingDateTime = newDate.Add(newTime),
                        CustomerName = customerName,
                        Request = RequestType.Rebooking
                    };
                    var existingReq = db.CustomerRequests
                        .Select(cr => cr.BookingId == editedBooking.BookingId);
                        
                    if (existingReq == null)
                    {
                        MessageBox.Show("You have already sent a booking request you may wait or contact the business to resolve");
                        return;
                    }
                    db.CustomerRequests.Add(newRequest);
                    db.SaveChanges();

                    MessageBox.Show("Rebooking request sent to the business successfully.");

                    CancelEdit_Click(null, null);
                    LoadBookings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending request: " + ex.Message);
            }
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            BookingsDataGrid.Visibility = Visibility.Visible;
            EditPanel.Visibility = Visibility.Collapsed;
            editedBooking = null;
        }

        private void CancelBookingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (editedBooking == null) return;

            var result = MessageBox.Show($"Are you sure you want to request a cancellation/refund for {editedBooking.Service.Name} on {editedBooking.Date.ToShortDateString()} at {editedBooking.Time}?",
                "Confirm Request", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var customerName = editedBooking.Customer.Name;

                    var newRequest = new CustomerRequest
                    {
                        BusinessId = editedBooking.BusinessId,
                        BookingId = editedBooking.BookingId,
                        CustomerId = editedBooking.CustomerId,
                        BookingDateTime = editedBooking.Date.Add(editedBooking.Time),
                        CustomerName = customerName,
                        Request = RequestType.Cancellation
                    };

                    db.CustomerRequests.Add(newRequest);
                    db.SaveChanges();

                    MessageBox.Show("Cancellation request sent to the business successfully.");
                    // Returns to previous page
                    CancelEdit_Click(null, null); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error sending request: " + ex.Message);
                }
            }
        }
    }
}
