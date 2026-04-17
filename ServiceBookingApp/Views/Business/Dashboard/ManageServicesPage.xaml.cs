using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ServiceBookingApp.Models;
using ServiceBookingApp.Helper;
using ServiceBookingApp.Data;
using System.Collections.Generic;

namespace ServiceBookingApp
{
    public partial class ManageServicesPage : Page
    {
        ServiceBookingContext db = new ServiceBookingContext();
        private Service _selectedService;

        public ManageServicesPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadServices();
        }
        
        private void LoadServices()
        {
            // Check if business session does not exist, if not show message and return. Else load services for the current business
            if (SessionManager.CurrentBusiness == null)
            {
                MessageBox.Show("No business session found. Please log in as a business to manage services.");
                return;
            } else {
                var services = db.Services
                                 .Where(s => s.BusinessId == SessionManager.CurrentBusiness.BusinessId)
                                 .ToList();
                if (services.Count == 0)
                {
                    var noServices = new List<Service> { new Service { Name = "No Services " } };
                    ServicesDataGrid.ItemsSource = noServices;
                }
                else
                {
                    ServicesDataGrid.ItemsSource = services;
                }
            }
        } // Method to load services for the current business session

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            // Trim inputs to remove leading/trailing whitespace
            NameBox.Text = NameBox.Text.Trim();
            PriceBox.Text = PriceBox.Text.Trim();
            DurationBox.Text = DurationBox.Text.Trim();
            // Validation for required fields and correct data types
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
            // Try creating a new service and save to database
            try
            {
                var newService = new Service
                {
                    BusinessId = SessionManager.CurrentBusiness.BusinessId,
                    Name = NameBox.Text,
                    Price = price,
                    Duration = TimeSpan.FromMinutes(durationMinutes),
                    Description = DescriptionBox.Text,
                    IsActive = IsActiveCheckBox.IsChecked ??  true
                };
                // Adding new service to database
                db.Services.Add(newService);
                db.SaveChanges();
                // Show success message, clear inputs and reload services
                MessageBox.Show("Service added successfully!");
                ClearInputs();
                LoadServices();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding service: {ex.Message}");
            }
        }  // Event handler for adding a new service
        
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
            UpdateIsActiveCheckBox.IsChecked = service.IsActive;

            // Show Update Panel n Hide Create Panel
            CreateServicePanel.Visibility = Visibility.Collapsed;
            UpdateServicePanel.Visibility = Visibility.Visible;
        }// Event handler for editing an existing service

        private void UpdateService_Click(object sender, RoutedEventArgs e)
        {
            // If no service is selected, return.
            if (_selectedService == null) return;
            // Validation for required fields and correct data types
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
                _selectedService.IsActive = UpdateIsActiveCheckBox.IsChecked ?? true;
                // Save changes to database
                db.SaveChanges();
                // Show success message and reload services
                MessageBox.Show("Service updated successfully!");
                LoadServices();
                // Switch back to create form
                CancelUpdate();
            }
            catch (Exception ex)
            {
                // Show error message if update fails
                MessageBox.Show($"Error updating service: {ex.Message}");
            }
        }// Event handler for updating an existing service

        private void CancelUpdateService_Click(object sender, RoutedEventArgs e)
        {
            CancelUpdate();
        }// Event handler for canceling the update process

        private void CancelUpdate()
        {
            // Clear selected service and reset form
            _selectedService = null;
            UpdateNameBox.Text = "";
            UpdatePriceBox.Text = "";
            UpdateDurationBox.Text = "";
            UpdateDescriptionBox.Text = "";
            UpdateServicePanel.Visibility = Visibility.Collapsed;
            CreateServicePanel.Visibility = Visibility.Visible;
        }// Method to reset the update form and switch back to the create service form

        private void DeleteService_Click(object sender, RoutedEventArgs e)
        {
            // Getting the button that was clicked
            var button = sender as Button;
            // Retrieving the service from the button's Tag property
            var service = button?.Tag as Service;
            // If the service is null, show an error message and return
            if (service == null)
            {
                MessageBox.Show("Invalid schedule selection.");
                return;
            }
            // Show confirmation dialog before deleting the service
            var result = MessageBox.Show(
                $"Are you sure you want to delete the service: '{service.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            // If the user confirms deletion, try to remove the service from the database
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
        } // Event handler for deleting a service

        private void ClearInputs()
        {
            NameBox.Text = "";
            PriceBox.Text = "";
            DurationBox.Text = "";
            DescriptionBox.Text = "";
        } // Method to clear input fields after adding a service
    }
}
