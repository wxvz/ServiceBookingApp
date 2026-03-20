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
