using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServiceBookingApp.Models;

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for ManageBookingsPage.xaml
    /// </summary>
    public partial class ManageBookingsPage : Page
    {
        public ServiceBookingContext db = new ServiceBookingContext();
        List<Booking> allBookings = new List<Booking>();
        private Booking editedBooking;

        public ManageBookingsPage()
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
            if (SessionManager.CurrentBusiness == null)
            {
                MessageBox.Show("No business is currently logged in.");
                return;
            }
            try
            {
                var rawBookings = db.Bookings
                    .Where(b => b.BusinessId == SessionManager.CurrentBusiness.BusinessId)
                    .ToList();

                var today = DateTime.Today;
                bool changed = false;

                foreach (var b in rawBookings) // Auto update status to Completed if the date has passed
                {
                    if ((b.Status == (BookingStatus)0 || b.Status == (BookingStatus)1) && b.Date < today) // Casts to pending and confirmed
                    {
                        b.Status = (BookingStatus)2; // Completed
                        changed = true;
                    }
                }

                if (changed) // So db isnt updated heavily 
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
            BookingStatusFilterCBX.SelectedIndex = 0; // Default to "All"
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

            // Apply filtering based on the selected booking status
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
                    {
                        BookingsDataGrid.ItemsSource = noBookingsSource;
                    }
                    else
                    {
                        BookingsDataGrid.ItemsSource = filteredBookings;
                    }
                }

            }
        } // Filter logic for the booking status filter combo box

        private void EditBooking_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Booking booking)  // Compares and casts(sets) at the same time very cool feature
            {
                editedBooking = booking;

                EditDatePicker.SelectedDate = booking.Date;
                EditTimeTextBox.Text = booking.Time.ToString(@"hh\:mm");
                
                EditStatusComboBox.SelectedIndex = (int)booking.Status;

                BookingsDataGrid.Visibility = Visibility.Collapsed;
                EditPanel.Visibility = Visibility.Visible;
            }
           
        } // Edit booking logic with conflict checking for confirmed and pending bookings

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
            BookingStatus newStatus = (BookingStatus)EditStatusComboBox.SelectedIndex;

            // Conflicting booking logic
            if (newStatus == BookingStatus.Confirmed || newStatus == BookingStatus.Pending)
            {
                bool conflict = db.Bookings.Any(b => b.BookingId != editedBooking.BookingId && 
                                                     b.ServiceId == editedBooking.ServiceId && 
                                                     b.Date == newDate && 
                                                     b.Time == newTime && 
                                                     (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending));
                if (conflict)
                {
                    MessageBox.Show("A conflicting booking already exists at the selected time.");
                    return;
                }
            }

            try
            {
                var b = db.Bookings.Find(editedBooking.BookingId); // Slightly faster somehow so ill research this more.
                if (b != null)
                {
                    b.Date = newDate;
                    b.Time = newTime;
                    b.Status = newStatus;
 
                    db.SaveChanges();

                    MessageBox.Show("Booking updated successfully.");
                }
                BookingsDataGrid.Visibility = Visibility.Visible;
                EditPanel.Visibility = Visibility.Collapsed;
                LoadBookings();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving: " + ex.Message);
            }
        } // Event Handler for Save Edit 

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            BookingsDataGrid.Visibility = Visibility.Visible;
            EditPanel.Visibility = Visibility.Collapsed;
        } // Event Handler for Cancel Edit

        private void RefundBookingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (editedBooking == null) return;

            var result = MessageBox.Show($"Are you sure you want to refund the booking for {editedBooking.Customer.Name} on {editedBooking.Date.ToShortDateString()} at {editedBooking.Time}?", "Confirm Refund", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var b = db.Bookings.Find(editedBooking.BookingId); // Find the booking in the database context
                    if (b != null)
                    {
                        db.Payments.Remove(b.Payment); 
                        db.Bookings.Remove(b);
                        db.SaveChanges();
                        LoadBookings();
                        MessageBox.Show("Booking deleted successfully.");
                        CancelEdit_Click(null, null); // Return to bookings list after deletion
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting booking: " + ex.Message);
                }
            }
        } // Event Handler for Delete Booking
    }
}
