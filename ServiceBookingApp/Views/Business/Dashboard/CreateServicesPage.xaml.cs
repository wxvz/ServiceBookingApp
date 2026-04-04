using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ServiceBookingApp
{
    public partial class CreateServicesPage : Page
    {
        ServiceBookingContext db = new ServiceBookingContext();

        public CreateServicesPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadServices();
        }

        private void LoadServices()
        {
            if (SessionManager.CurrentBusiness == null)
            {
                MessageBox.Show("No business session found. Please log in as a business to manage services.");
                return;
            } else {
                var services = db.Services
                                 .Where(s => s.BusinessId == SessionManager.CurrentBusiness.BusinessId)
                                 .ToList();
                ServicesDataGrid.ItemsSource = services;
            }
        }

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(PriceBox.Text) ||
                string.IsNullOrWhiteSpace(DurationBox.Text))
            {
                MessageBox.Show("Please fill in all required fields (Name, Price, Duration).");
                return;
            }
            if (!decimal.TryParse(PriceBox.Text, out decimal price))
            {
                MessageBox.Show("Invalid Price.");
                return;
            }
            if (!int.TryParse(DurationBox.Text, out int durationMinutes))
            {
                MessageBox.Show("Invalid Duration (minutes).");
                return;
            }
            try
            {
                var newService = new Service
                {
                    BusinessId = SessionManager.CurrentBusiness.BusinessId,
                    Name = NameBox.Text,
                    Price = price,
                    Duration = TimeSpan.FromMinutes(durationMinutes),
                    Description = DescriptionBox.Text
                };

                db.Services.Add(newService);
                db.SaveChanges();

                MessageBox.Show("Service added successfully!");
                ClearInputs();
                LoadServices();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding service: {ex.Message}");
            }
        }

        private void EditService_Click(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked
            var button = sender as Button;
            // Retrieve the service from the buttons Tag property
            var service = button?.Tag as Service;

            if (service == null)
            {
                MessageBox.Show("Invalid service selection.");
                return;
            }
        }

        private void DeleteService_Click(object sender, RoutedEventArgs e)
        {
            // Getting the button that was clicked
            var button = sender as Button;
            
            var service = button?.Tag as Service;

            if (service == null)
            {
                MessageBox.Show("Invalid schedule selection.");
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete the service: '{service.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    db.Services.Remove(service);
                    db.SaveChanges();
                    MessageBox.Show("Service deleted successfully!");
                    // Reload services
                    LoadServices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting service: {ex.Message}");
                }
            }
        }

        private void ClearInputs()
        {
            NameBox.Text = "";
            PriceBox.Text = "";
            DurationBox.Text = "";
            DescriptionBox.Text = "";
        }
    }
}
