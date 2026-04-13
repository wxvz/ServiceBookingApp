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

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for ViewBookingsPage.xaml
    /// </summary>
    public partial class ViewBookingsPage : Page
    {
        public ServiceBookingContext db = new ServiceBookingContext();

        public ViewBookingsPage()
        {
            InitializeComponent();
        }

        private void ViewBookingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUpcomingBookings();
        }

        private void LoadUpcomingBookings()
        {
            if (SessionManager.CurrentBusiness == null) return;

            try
            {
                // Load bookings for the next 3 months
                var today = DateTime.Today;
                var threeMonthsFromNow = today.AddMonths(3);

                // Only show Pending and Confirmed bookings
                var upcoming = db.Bookings
                    .Where(b => b.BusinessId == SessionManager.CurrentBusiness.BusinessId &&
                                b.Date >= today && b.Date <= threeMonthsFromNow &&
                                (b.Status == (BookingStatus)0 || b.Status == (BookingStatus)1)) 
                    .OrderBy(b => b.Date).ThenBy(b => b.Time) // Sort by date then time
                    .ToList();

                UpcomingBookingsDataGrid.ItemsSource = upcoming;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bookings: " + ex.Message);
            }
        }
       
    }
}
