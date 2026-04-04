using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ServiceBookingApp
{
    public partial class CreateServicesPage : Page
    {
        ServiceBookingContext db = new ServiceBookingContext();
        private Service _selectedService;

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

            _selectedService = service;

            // Populate textboxes
            UpdateNameBox.Text = service.Name;
            UpdatePriceBox.Text = service.Price.ToString("F2");
            UpdateDurationBox.Text = service.Duration.TotalMinutes.ToString();
            UpdateDescriptionBox.Text = service.Description;

            // Show Update Panel n Hide Create Panel
            CreateServicePanel.Visibility = Visibility.Collapsed;
            UpdateServicePanel.Visibility = Visibility.Visible;
        }

        private void UpdateService_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedService == null) return;

            // Validation
            if (string.IsNullOrWhiteSpace(UpdateNameBox.Text) ||
                string.IsNullOrWhiteSpace(UpdatePriceBox.Text) ||
                string.IsNullOrWhiteSpace(UpdateDurationBox.Text))
            {
                MessageBox.Show("Please fill in all required fields (Name, Price, Duration).");
                return;
            }
            if (!decimal.TryParse(UpdatePriceBox.Text, out decimal price))
            {
                MessageBox.Show("Invalid Price.");
                return;
            }
            if (!int.TryParse(UpdateDurationBox.Text, out int durationMinutes))
            {
                MessageBox.Show("Invalid Duration (minutes).");
                return;
            }

            try
            {
                // Updating service
                _selectedService.Name = UpdateNameBox.Text;
                _selectedService.Price = price;
                _selectedService.Duration = TimeSpan.FromMinutes(durationMinutes);
                _selectedService.Description = UpdateDescriptionBox.Text;

                db.SaveChanges();

                MessageBox.Show("Service updated successfully!");
                LoadServices();

                // Switch back to create form
                CancelUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating service: {ex.Message}");
            }
        }

        private void CancelUpdateService_Click(object sender, RoutedEventArgs e)
        {
            CancelUpdate();
        }

        private void CancelUpdate()
        {
            _selectedService = null;
            UpdateNameBox.Text = "";
            UpdatePriceBox.Text = "";
            UpdateDurationBox.Text = "";
            UpdateDescriptionBox.Text = "";

            UpdateServicePanel.Visibility = Visibility.Collapsed;
            CreateServicePanel.Visibility = Visibility.Visible;
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
