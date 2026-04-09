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
        public SeriesCollection MonthlyRevenueChart { get; set; }
        public string[] MonthLabels { get; set; }
        // Formatter for currency values in the charts
        public Func<double, string> CurrencyFormatter { get; set; } 

        // Database context
        ServiceBookingContext db = new ServiceBookingContext();
        public BusinessDashboard()
        {
            InitializeComponent();
        }

        private void ChartContext()
        {
            DataContext = null;
            DataContext = this;
        } // Set the DataContext for chart data binding 
        private void LoadChartData(int totalBookings)
        {
            
            // Initialize the chart data collection
            ServiceBookingsChart = new SeriesCollection();
            MonthlyRevenueChart = new SeriesCollection();

            // PIE CHART DATA - Bookings per Service 
            var serviceStats = db.Services
                .Where(s => s.BusinessId == SessionManager.CurrentBusiness.BusinessId)
                .Select(s => new
                {
                    ServiceName = s.Name,
                    BookingCount = s.Bookings.Count()
                }).ToList();

            // If there are no services, add a placeholder entry to the chart
            if (!serviceStats.Any())
            {
                ServiceBookingsChart.Add(new PieSeries
                {
                    Title = "No Services",
                    Values = new ChartValues<int> { 1 },
                    DataLabels = true
                });
                ChartContext(); // Refresh the chart context to display the placeholder
            }
            // If there are no bookings, add a placeholder entry to the chart
            if (totalBookings == 0)
            {
                ServiceBookingsChart.Add(new PieSeries
                {
                    Title = "No Bookings",
                    Values = new ChartValues<int> { 1 },
                    DataLabels = true,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4C5F6B"))

                });
                ChartContext();
            }
            // Add data to the chart
            else
            {
                //Array of custom colours from coolors.co (pERSONALLY procured BY ME)
                string[] colours = { 
                    "#C2D3CD", "#689689", "#96E8BC", "#B6F9C9", "#E5E7E6", "#C9FFE2", "#9FA4A9",
                    "#ADA8B6", "#A2A79E", "#B7B7B7", "#C7CFFD", "#82ABA1", "#909590", "#E1DEE9"
                }; 
                int colourIndex = 0;
                // for each  Stat in Service Service Add to chart
                foreach (var stats in serviceStats)
                {
                    if (stats.BookingCount > 0)
                    {
                        ServiceBookingsChart.Add(new PieSeries
                        {
                            Title = stats.ServiceName,
                            Values = new ChartValues<int> { stats.BookingCount },
                            DataLabels = true,
                            // Apply the color and increment the index (loop back if we have more services than colours)
                            Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colours[colourIndex % colours.Length])),
                            // Set label color for better visibility
                            Foreground = Brushes.Black
                        });
                        colourIndex++;
                    }
                }
                ChartContext();
            }
            // BAR CHART DATA - Monthly Revenue
            // Load monthly revenue data 
            var payments = db.Payments
                .Where(p => p.BusinessId == SessionManager.CurrentBusiness.BusinessId)
                .ToList();
            // Group payments by month and calculate total revenue for each month
            var monthlyRevenue = payments
                .GroupBy(p => new
                {
                    p.PaymentDate.Year, p.PaymentDate.Month 
                })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    // Create a month name like "Jan 2024" for display
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    totalRevenueTBX = g.Sum(p => p.Amount)
                })
                .ToList();
            // If there are no payments, add a placeholder entry to the chart
            if (!monthlyRevenue.Any())
            {
                MonthLabels = new[] { "No Revenue" };
                MonthlyRevenueChart.Add(new ColumnSeries
                {
                    Title = "Revenue",
                    Values = new ChartValues<decimal> { 0 }
                });
                ChartContext();
            }
            else // Add data to the chart
            {
                MonthLabels = monthlyRevenue.Select(m => m.MonthName).ToArray();
                MonthlyRevenueChart.Add(new ColumnSeries
                {
                    Title = "Monthly Revenue",
                    Values = new ChartValues<decimal>(monthlyRevenue.Select(m => m.totalRevenueTBX)),
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#83A0A0")),
                    DataLabels = true
                });
                ChartContext();
            }
            CurrencyFormatter = value => $"€{value:F2}"; // Format values as euro with 2 decimal places
            ChartContext(); // Reload DataContext For LAst Time with all values
           
        } // Fetchs Business Data and displays as charts using live chart extension
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
            LoadChartData(totalBookings);
        } // Loads Business Data from Session On Window Loaded 
        private void ServicesButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the Manage Services page within the dashboard frame
            DashboardFrame.Navigate(new ManageServicesPage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        } // Event Handler for Service's Button
        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the main dashboard page within the dashboard frame
            DashboardFrame.Visibility = Visibility.Hidden;
            ShowDashboardContent();
            LoadBusiness(null, null); // Refresh stats
        }  // Event Handler Dashboard Button
        private void HideDashboardContent()
        {
            DashboardHeaderTBX.Visibility = Visibility.Hidden;
            totalBookingsTBX.Visibility = Visibility.Hidden;
            totalRevenueTBX.Visibility = Visibility.Hidden;
            BarChartStats.Visibility = Visibility.Hidden;
            PieChartStats.Visibility = Visibility.Hidden;
        } // Hide Main Dashboard Content
        private void ShowDashboardContent()
        {
            DashboardHeaderTBX.Visibility = Visibility.Visible;
            totalBookingsTBX.Visibility = Visibility.Visible;
            totalRevenueTBX.Visibility = Visibility.Visible;
            BarChartStats.Visibility = Visibility.Visible;
            PieChartStats.Visibility = Visibility.Visible;
        } // Show Main Dashboard Content
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
        }  // Event Handler for Logout Button
        private void ManageServices_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the Manage Services page within the dashboard frame
            DashboardFrame.Navigate(new ManageServicesPage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        } // Event Handler for Manage Service button
        private void ManageSchedules_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to the Manage Service Schedule page within the dashboard frame
            DashboardFrame.Navigate(new ManageServiceSchedulePage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        } // Event HandlerManage Schedule's Button
        private void ViewProfile_Click(object sender, RoutedEventArgs e)
        {
            DashboardFrame.Navigate(new ViewProfilePage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        }
        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            DashboardFrame.Navigate(new EditProfilePage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        }
        private void ViewBookings_Click(object sender, RoutedEventArgs e)
        {
            DashboardFrame.Navigate(new ViewBookingsPage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        }
        private void ManageBookings_Click(object sender, RoutedEventArgs e)
        {
            DashboardFrame.Navigate(new ManageBookingsPage());
            DashboardFrame.Visibility = Visibility.Visible;
            HideDashboardContent();
        }
    }
}
