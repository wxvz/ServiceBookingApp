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
    /// Interaction logic for CustomerDashboard.xaml
    /// </summary>
    public partial class CustomerDashboard : Page
    {
        public CustomerDashboard()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void LoadCustomer()
        {
            // Load customer data and display on dashboard
            SessionManager.LoadSession();
           
            if (SessionManager.CurrentCustomer == null)
            {
                MessageBox.Show("No customer logged in.");
                return;
            }

        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {

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
