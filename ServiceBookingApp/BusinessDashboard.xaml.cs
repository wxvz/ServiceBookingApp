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
using System.Windows.Shapes;

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for BusinessDashboard.xaml
    /// </summary>
    public partial class BusinessDashboard : Page
    {
        ServiceBookingContext db = new ServiceBookingContext();
        public BusinessDashboard()
        {
            InitializeComponent();
        }

        private void LoadBusiness(object sender, RoutedEventArgs e)
        { 

            // Load the current business from the session and display its name
            SessionManager.LoadSession();
            businessName.Text = SessionManager.CurrentBusiness.Name;

            // Total bookings
            int totalBookings = db.Bookings.Count(b => b.BusinessId == SessionManager.CurrentBusiness.BusinessId);

            //Total revenue
            decimal totalRevenue = db.Payments
                .Where(p => p.BusinessId == SessionManager.CurrentBusiness.BusinessId)
                .Sum(p => (decimal?)p.Amount) ?? 0;

            // Display the details
            totalBookingsTBX.Text = $"Total Bookings: {totalBookings}";

            totalRevenueTBX.Text = $"Total Revenue: €{totalRevenue:F2}";
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Logout();
            MessageBox.Show("You have been logged out.");

            // Navigate back to the login page  
            NavigationService.Navigate(new BusinessLogin());
        }
    }
}
