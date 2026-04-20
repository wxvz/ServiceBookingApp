using ServiceBookingApp.Views.Customer.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ServiceBookingApp.Models;
using ServiceBookingApp.Views.Customer.Booking_Flow;
using ServiceBookingApp.Data;
using ServiceBookingApp.Helper;

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for CustomerDashboard.xaml
    /// </summary>
    public partial class CustomerDashboard : Page
    {
        private ServiceBookingContext db = new ServiceBookingContext();

        public CustomerDashboard()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCustomer();
        }
        private void LoadCustomer()
        {
            // Load customer data and display on dashboard
            SessionManager.LoadSession();
           
            if (SessionManager.CurrentCustomer == null)
            {
                MessageBox.Show("No customer logged in.");
                this.NavigationService.Navigate(new CustomerLogin());
                return;
            }
            var customer = SessionManager.CurrentCustomer;
            string[] fullName = customer.Name.Split(' ');
            string firstName = fullName[0];

            customerNameTBX.Text = $"Hi, {firstName}.";
        }
        private void EditProfile_Btn(object sender, RoutedEventArgs e)
        {
            DashboardFrame.Navigate(new EditProfile());
            DashboardFrame.Visibility = Visibility.Visible;
            HomeContent.Visibility = Visibility.Hidden;
        }
        private void BookService_Btn(object sender, RoutedEventArgs e)
        {
            DashboardFrame.Navigate(new BookServicePage());
            DashboardFrame.Visibility = Visibility.Visible;
            HomeContent.Visibility = Visibility.Hidden;
        }
        private void ManageBookings_Btn(object sender, RoutedEventArgs e)
        {
            DashboardFrame.Navigate(new CustomerBookingsPage());
            DashboardFrame.Visibility = Visibility.Visible;
            HomeContent.Visibility = Visibility.Hidden;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Log Out?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
            // If user clicks no return 
            if (result != MessageBoxResult.Yes) return;
            // Clear session and log out
            SessionManager.LogOut();
            MessageBox.Show("You have been logged out.");
            // Navigate back to the login page  
            NavigationService.Navigate(new HomePage());
        }  // Event Handler for Logout Button

        
    }
}
