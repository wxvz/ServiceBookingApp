using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ServiceBookingApp.Views.Customer.Booking_Flow
{
    /// <summary>
    /// Interaction logic for BusinessBookingProfile.xaml
    /// </summary>
    public partial class BusinessBookingProfile : Page
    {
        private readonly int _businessId;
        private ServiceBookingContext db = new ServiceBookingContext();

        public BusinessBookingProfile(int businessId)
        {
            InitializeComponent();
            _businessId = businessId;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBusinessProfile();
        }

        private void LoadBusinessProfile()
        {
            try
            {
                var business = db.Businesses.FirstOrDefault(b => b.BusinessId == _businessId);
                var services = db.Services.Where(s => s.BusinessId == _businessId && s.IsActive).ToList();

                if (business != null)
                {
                    BusinessNameText.Text = business.Name;
                    BusinessDescriptionText.Text = business.Description;
                    BusinessContactText.Text = $"{business.Address} | {business.PhoneNumber} | {business.Email}";
                }

                if (services.Count > 0)
                {
                    ServicesItemsControl.ItemsSource = services;
                    ServicesItemsControl.Visibility = Visibility.Visible;
                    NoServicesText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ServicesItemsControl.Visibility = Visibility.Collapsed;
                    NoServicesText.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading business profile: {ex.Message}");
            }
        }

        private void BookService_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int serviceId)
            {
                NavigationService.Navigate(new ConfirmBookingPage(serviceId));
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
