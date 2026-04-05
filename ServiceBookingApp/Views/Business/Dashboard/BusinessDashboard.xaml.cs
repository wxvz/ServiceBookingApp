using LiveCharts.Wpf;
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
using LiveCharts;

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for BusinessDashboard.xaml
    /// </summary>
    public partial class BusinessDashboard : Page
    {
        // Chart data properties
        public SeriesCollection ServiceBookingsChart { get; set; }

        // Database context
        ServiceBookingContext db = new ServiceBookingContext();
        public BusinessDashboard()
        {
            InitializeComponent();
        }
        private void LoadChartData()
        {
            // Initialize the chart data collection
            ServiceBookingsChart = new SeriesCollection();
            
            // Load Services for chart 
            var serviceStats = db.Services
                .Where(s => s.BusinessId == SessionManager.CurrentBusiness.BusinessId)
                .Select(s => new
                {
                    ServiceName = s.Name,
                    BookingCount = s.Bookings.Count()
                }).ToList();
            foreach (var stats in serviceStats)
            {
                ServiceBookingsChart.Add(new PieSeries
                {
                    Title = stats.ServiceName,
                    Values = new ChartValues<int> { stats.BookingCount },
                    DataLabels = true
                });
            }
            // Set the DataContext for data binding
            DataContext = this;
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
            // Display the business details
            totalBookingsTBX.Text = $"Total Bookings: {totalBookings}";
            totalRevenueTBX.Text = $"Total Revenue: €{totalRevenue:F2}";
            // Load chart data
            LoadChartData();
        }
        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the Manage Services page within the dashboard frame
            DashboardFrame.Navigate(new ManageServicesPage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        }
        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the main dashboard page within the dashboard frame
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
            // Show dashboard specific elements
            totalBookingsTBX.Visibility = Visibility.Visible;
            totalRevenueTBX.Visibility = Visibility.Visible;
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Log Out?","Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
            // If user clicks no return 
            if (result != MessageBoxResult.Yes) return;
            // Clear session and log out
            SessionManager.LogOut();
            MessageBox.Show("You have been logged out.");
            // Navigate back to the login page  
            NavigationService.Navigate(new HomePage());
        }
        private void ManageServices_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the Manage Services page within the dashboard frame
            DashboardFrame.Navigate(new ManageServicesPage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        }
        private void ManageSchedules_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the Manage Service Schedule page within the dashboard frame
            DashboardFrame.Navigate(new ManageServiceSchedulePage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        }
    }
}
