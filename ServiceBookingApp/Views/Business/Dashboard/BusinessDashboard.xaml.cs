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

        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            DashboardFrame.Navigate(new CreateServicesPage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            DashboardFrame.Visibility = Visibility.Hidden;
            ShowDashboardContent();
            LoadBusiness(null, null); // Refresh stats
        }

        private void HideDashboardContent()
        {
            totalBookingsTBX.Visibility = Visibility.Hidden;
            totalRevenueTBX.Visibility = Visibility.Hidden;
            // Hide other dashboard specific elements
        }

        private void ShowDashboardContent()
        {
            totalBookingsTBX.Visibility = Visibility.Visible;
            totalRevenueTBX.Visibility = Visibility.Visible;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Logout();
            MessageBox.Show("You have been logged out.");

            // Navigate back to the login page  
            NavigationService.Navigate(new HomePage());
        }
    
        private void ManageServices_Click(object sender, RoutedEventArgs e)
        {
            DashboardFrame.Navigate(new ManageServicesPage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        }

        private void ManageSchedules_Click(object sender, RoutedEventArgs e)
        {
            DashboardFrame.Navigate(new ManageServiceSchedulePage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        }
    }
}
