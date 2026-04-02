using System.Windows;
using System.Windows.Controls;

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }
        private void SessionCheck(object sender, RoutedEventArgs e)
        {
            SessionManager.LoadSession(); // Load session data (if any) when the page is loaded

            // Check if theres an active session for either a customer or a business
            // and navigate to the appropriate dashboard
            if (SessionManager.CurrentCustomer != null)
            {
                this.NavigationService.Navigate(new CustomerDashboard());
            }
            else if (SessionManager.CurrentBusiness != null)
            {
                this.NavigationService.Navigate(new BusinessDashboard());
            }
        }

        private void customerBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new CustomerLogin());
        }

        private void businessBtn_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new BusinessLogin());
        }

        
    }
}
