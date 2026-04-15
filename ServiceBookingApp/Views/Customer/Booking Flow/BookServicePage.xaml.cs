using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ServiceBookingApp.Views.Customer.Booking_Flow
{
    /// <summary>
    /// Interaction logic for BookServicePage.xaml
    /// </summary>
    public partial class BookServicePage : Page
    {
        private ServiceBookingContext db = new ServiceBookingContext();

        public BookServicePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBusinesses();
        }

        private void LoadBusinesses()
        {
            try
            {
                var businesses = db.Businesses.ToList();
                if (businesses.Count > 0)
                {
                    BusinessesItemsControl.ItemsSource = businesses;
                    NoBusinessesText.Visibility = Visibility.Collapsed;
                    BusinessesItemsControl.Visibility = Visibility.Visible;
                }
                else
                {
                    NoBusinessesText.Visibility = Visibility.Visible;
                    BusinessesItemsControl.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading businesses: {ex.Message}");
            }
        }

        private void ViewServices_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int businessId)
            {
                this.NavigationService.Navigate(new BusinessBookingProfile(businessId));
            }
        }
    }
}
