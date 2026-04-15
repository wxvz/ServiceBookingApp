using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ServiceBookingApp.Models;
using ServiceBookingApp.Data;
using ServiceBookingApp.Helper;

namespace ServiceBookingApp.Views.Business.Dashboard
{
    /// <summary>
    /// Interaction logic for CustomerRequests.xaml
    /// </summary>
    public partial class CustomerRequests : Page
    {
        public ServiceBookingContext db = new ServiceBookingContext();
        private List<CustomerRequest> allRequests = new List<CustomerRequest>();
        private CustomerRequest selectedRequest;

        public CustomerRequests()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRequests();
            LoadFilterOptions();
        }

        private void LoadRequests()
        {
            if (SessionManager.CurrentBusiness == null)
            {
                MessageBox.Show("No business is currently logged in.");
                return;
            }

            try
            {
                db = new ServiceBookingContext();
                allRequests = db.CustomerRequests
                    .Where(cr => cr.BusinessId == SessionManager.CurrentBusiness.BusinessId)
                    .ToList();

                FilterByRequestType();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading requests: " + ex.Message);
            }
        }

        private void LoadFilterOptions()
        {
            string[] reqTypes = { "-- All --", "Refund", "Rebooking", "Cancellation" };
            RequestTypeFilterCBX.ItemsSource = reqTypes;
            RequestTypeFilterCBX.SelectedIndex = 0;
        }

        private void RequestTypeFilterCBX_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterByRequestType();
            var noRequestsSource = new List<CustomerRequest> { new CustomerRequest { CustomerName = "No active requests." } };
            if (RequestTypeFilterCBX.SelectedItem == null || allRequests == null) return;

            if (allRequests.Count == 0)
            {
                RequestsDataGrid.ItemsSource = noRequestsSource;
                return;
            }
        }

        private void FilterByRequestType()
        {
            string[] noRequestsSource = { "No active requests." };
            if (RequestTypeFilterCBX.SelectedItem == null || allRequests == null) return;

            if (allRequests.Count == 0)
            {
                RequestsDataGrid.ItemsSource = noRequestsSource;
                return;
            }

            string selectedStatus = RequestTypeFilterCBX.SelectedItem.ToString();
            if (selectedStatus == "-- All --")
            {
                RequestsDataGrid.ItemsSource = allRequests;
            }
            else
            {
                if (Enum.TryParse(selectedStatus, out RequestType parsedType))
                {
                    var filteredRequests = allRequests
                        .Where(r => r.Request == parsedType)
                        .ToList();

                    if (filteredRequests.Count == 0)
                        RequestsDataGrid.ItemsSource = noRequestsSource;
                    else
                        RequestsDataGrid.ItemsSource = filteredRequests;
                }
            }
        }

        private void ActionRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CustomerRequest req)
            {
                selectedRequest = req;
                
                RequestDetailsTextBlock.Text = $"Customer: {req.CustomerName}\n" +
                                               $"Booking Date/Time: {req.BookingDateTime.ToString("f")}\n" +
                                               $"Type of Request: {req.Request}";

                RequestsDataGrid.Visibility = Visibility.Collapsed;
                ActionPanel.Visibility = Visibility.Visible;
            }
        }

        private void ApproveRequest_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRequest == null) return;

            try
            {
                var req = db.CustomerRequests.Find(selectedRequest.Id);
                if (req != null)
                {
                    // Fetch real booking explicitly from DB here if needed
                    var relatedBooking = db.Bookings.Find(req.BookingId);

                    switch (req.Request)
                    {
                        case RequestType.Cancellation:
                            if (relatedBooking != null) relatedBooking.Status = BookingStatus.Cancelled;
                            MessageBox.Show($"Booking for {req.CustomerName} has been cancelled.");
                            break;
                            
                        case RequestType.Refund: // Assuming they cancel on refund request as well, change if specific financial api is built.
                            if (relatedBooking != null) relatedBooking.Status = BookingStatus.Cancelled;
                            MessageBox.Show($"Refund process initiated for {req.CustomerName} and booking cancelled.");
                            break;

                        case RequestType.Rebooking:
                            MessageBox.Show($"Rebooking contact info sent via email to {req.CustomerName}."); // Simulated rebooking process manually handled by business outside app or via a built messaging feature.
                            break;
                    }

                    // Request processed, remove notification request from database
                    db.CustomerRequests.Remove(req);
                    db.SaveChanges();
                }

                CancelAction_Click(null, null);
                LoadRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing request: " + ex.Message);
            }
        }

        private void DismissRequest_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRequest == null) return;

            var result = MessageBox.Show($"Are you sure you want to dismiss the {selectedRequest.Request} request from {selectedRequest.CustomerName}? The original booking will not be changed.", "Confirm Dismissal", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var req = db.CustomerRequests.Find(selectedRequest.Id);
                    if (req != null)
                    {
                        db.CustomerRequests.Remove(req);
                        db.SaveChanges();
                        MessageBox.Show("Request dismissed.");
                    }

                    CancelAction_Click(null, null);
                    LoadRequests();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error dismissing request: " + ex.Message);
                }
            }
        }

        private void CancelAction_Click(object sender, RoutedEventArgs e)
        {
            RequestsDataGrid.Visibility = Visibility.Visible;
            ActionPanel.Visibility = Visibility.Collapsed;
            selectedRequest = null;
        }
    }
}
