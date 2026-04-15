using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ServiceBookingApp.Models;
using ServiceBookingApp.Data;
using ServiceBookingApp.Helper;

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for EditService.xaml
    /// </summary>
    public partial class ManageServiceSchedulePage : Page
    {
        private ServiceBookingContext db = new ServiceBookingContext();
        private List<ServiceSchedule> allSchedules = new List<ServiceSchedule>();
        
        public ManageServiceSchedulePage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadServices();
            LoadDays();
            LoadSchedules();
        }

        private void LoadSchedules()
        {
            // Ensure we have a business session before trying to load schedules
            if (SessionManager.CurrentBusiness == null)
            {
                MessageBox.Show("No business session found. Please log in as a business to manage schedules.");
                return;
            }

            try
            {
                // Load all schedules for the current business and include related service data for display
                allSchedules = db.ServiceSchedules
                    .Where(s => s.Service.BusinessId == SessionManager.CurrentBusiness.BusinessId)
                    .Include(s => s.Service)
                    .ToList();

                FilterSchedulesByService();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedules: {ex.Message}");
            }
        }

        private void LoadServices()
        {
            // Ensure we have a business session before trying to load services
            if (SessionManager.CurrentBusiness == null) return;
            try
            {
                // Load all services for the current business to populate the filter combo box and the service selection combo box
                var services = db.Services
                    .Where(s => s.BusinessId == SessionManager.CurrentBusiness.BusinessId)
                    .ToList();
                ServiceFilterComboBox.ItemsSource = services;
                // Add a dummy Service object to act as the placeholder for the "Select a Service" option in the filter combo box
                var filterServices = new List<Service>
                {
                    new Service { Name = "--- Select a Service ---", ServiceId = -1 }
                };
                // Add actual services to the filter list
                filterServices.AddRange(services);
                ServiceFilterComboBox.ItemsSource = filterServices;
                ServiceFilterComboBox.SelectedIndex = 0; // Select placeholder by default
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading services: {ex.Message}");
            }
        }

        private void ServiceFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterSchedulesByService();
        } 

        private void FilterSchedulesByService()
        {
            // If the filter combo box or schedules list is null. Cannot filter so return.
            if (ServiceFilterComboBox == null || allSchedules == null) return;
            try
            {
                // Get the selected value from the service filter combo box.
                var selectedValue = ServiceFilterComboBox.SelectedValue;
                // If the selected value is null or the combo box placeholder is selected, do not show any schedules
                if (selectedValue == null || (int)selectedValue == -1)
                {
                    SchedulesDataGrid.ItemsSource = null;
                }
                else
                {
                    // Filter schedules by selected service
                    var filteredSchedules = allSchedules
                        .Where(s => s.ServiceId == (int)selectedValue)
                        .ToList();
                    SchedulesDataGrid.ItemsSource = filteredSchedules;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering schedules: {ex.Message}");
            }
        } // Method to filter the displayed schedules based on the selected service in the filter combo box

        private void LoadDays()
        {
            // Load days of the week into the DayComboBox
            var days = Enum.GetValues(typeof(DayOfWeek))
                .Cast<DayOfWeek>()
                .Select(d => d.ToString())
                .ToList();
            DayComboBox.ItemsSource = days;
            UpdateDayComboBox.ItemsSource = days;
        } // Method to load days of the week into the day selection combo boxes

        private void SaveSchedule_Click(object sender, RoutedEventArgs e)
        {
            // If "Select a service" or null is selected, we cannot add a schedule
            if (ServiceFilterComboBox.SelectedIndex == 0) 
            {
                MessageBox.Show("Please select a service to add a schedule for.");
                return;
            }
            if (ServiceFilterComboBox.SelectedItem == null) 
            {
                MessageBox.Show("Please select a service.");
                return;
            }
            if (DayComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a day.");
                return;
            }
            if (!TimeSpan.TryParse(StartTimeBox.Text, out TimeSpan startTime))
            {
                MessageBox.Show("Invalid start time format. Use HH:MM format.");
                return;
            }
            if (!TimeSpan.TryParse(EndTimeBox.Text, out TimeSpan endTime))
            {
                MessageBox.Show("Invalid service end time format. Use HH:MM format.");
                return;
            }
            if (endTime <= startTime)
            {
                MessageBox.Show("Service end time must be after start time.");
                return;
            }
            try
            {
                // Get the selected service ID and day of week
                int selectedServiceId = (int)ServiceFilterComboBox.SelectedValue;
                var selectedDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), DayComboBox.SelectedItem.ToString());
                // Check for conflicts with existing active schedules for the same service and day
                var conflictingSchedule = db.ServiceSchedules
                    .Where(s => s.ServiceId == selectedServiceId &&
                               s.DayOfWeek == selectedDay &&
                               s.IsActive &&
                               ((startTime >= s.StartTime && startTime < s.EndTime) ||
                                (endTime > s.StartTime && endTime <= s.EndTime) ||
                                (startTime <= s.StartTime && endTime >= s.EndTime)))
                    .FirstOrDefault();
                // If a conflicting schedule exists, show an error message and do not add the new schedule
                if (conflictingSchedule != null)
                {
                    MessageBox.Show("An existing active schedule exists for the same service.");
                    return;
                }
                // If no conflicts, create and save the new schedule
                var newSchedule = new ServiceSchedule
                {
                    ServiceId = selectedServiceId,
                    DayOfWeek = selectedDay,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsActive = ActiveCheckBox.IsChecked ?? true // catch nulls
                };
                // Add the new schedule to the database and save changes
                db.ServiceSchedules.Add(newSchedule);
                db.SaveChanges();
                // Show success message, clear input fields, and reload schedules to show the new entry
                MessageBox.Show("Schedule added successfully!");
                ClearInputs();
                LoadSchedules();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding schedule: {ex.Message}");
            }
        }// Event handler for saving a new schedule to the database

        private void EditSchedule_Click(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked
            var button = sender as Button;
            // Retrieve the schedule from the buttons Tag property
            var schedule = button?.Tag as ServiceSchedule;
            // If the schedule is null, show an error message and return
            if (schedule == null)
            {
                MessageBox.Show("Invalid schedule selection.");
                return;
            }
            // Show the Update panel and fill the current values for the selected schedule
            UpdateSchedulePanel.Visibility = Visibility.Visible;
            UpdateSchedulePanel.Tag = schedule;
            UpdateDayComboBox.SelectedItem = schedule.DayOfWeek.ToString();
            UpdateStartTimeBox.Text = schedule.StartTime.ToString(@"hh\:mm");
            UpdateEndTimeBox.Text = schedule.EndTime.ToString(@"hh\:mm");
            UpdateActiveCheckBox.IsChecked = schedule.IsActive;
        } // Event handler for showing the update panel and populating it with the selected schedules current values

        private void CancelUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateSchedulePanel.Visibility = Visibility.Collapsed;
            UpdateSchedulePanel.Tag = null;
        }// Event handler for canceling the update of a schedule

        private void UpdateScheduleDb_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve the schedule being updated from the Update panel's Tag property
            var schedule = UpdateSchedulePanel.Tag as ServiceSchedule;
            if (schedule == null) return;

            if (UpdateDayComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a day.");
                return;
            }
            if (!TimeSpan.TryParse(UpdateStartTimeBox.Text, out TimeSpan startTime))
            {
                MessageBox.Show("Invalid start time format. Use HH:MM format.");
                return;
            }
            if (!TimeSpan.TryParse(UpdateEndTimeBox.Text, out TimeSpan endTime))
            {
                MessageBox.Show("Invalid service end time format. Use HH:MM format.");
                return;
            }
            if (endTime <= startTime)
            {
                MessageBox.Show("Service end time must be after start time.");
                return;
            }
            try
            {
                // Get the selected service ID from the schedule being updated and the selected day of week from the update form
                int selectedServiceId = schedule.ServiceId;
                var selectedDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), UpdateDayComboBox.SelectedItem.ToString());
                var serviceActivated = UpdateActiveCheckBox.IsChecked;

                // Check for conflicts with existing active schedules for the same service and day, excluding the current schedule being updated
                var conflictingSchedule = db.ServiceSchedules
                    .Where(s => s.ServiceId == selectedServiceId &&
                               s.DayOfWeek == selectedDay &&
                               s.IsActive == serviceActivated &&
                               ((startTime >= s.StartTime && startTime < s.EndTime) ||
                                (endTime > s.StartTime && endTime <= s.EndTime) ||
                                (startTime <= s.StartTime && endTime >= s.EndTime)))
                    .FirstOrDefault();
                // If a conflicting schedule exists, show an error message and do not add the updated schedule
                if (conflictingSchedule != null)
                {
                    MessageBox.Show("An existing active schedule exists for the same service.");
                    return;
                }
                // If no conflicts, update the schedule with the new values and save changes to the database
                schedule.DayOfWeek = selectedDay;
                schedule.StartTime = startTime;
                schedule.EndTime = endTime;
                schedule.IsActive = UpdateActiveCheckBox.IsChecked ?? true;
                db.SaveChanges();
                // Show success message, hide the update panel, and reload schedules to show the updated entry
                MessageBox.Show("Schedule updated successfully!");
                UpdateSchedulePanel.Visibility = Visibility.Collapsed;
                UpdateSchedulePanel.Tag = null;
                LoadSchedules();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating schedule: {ex.Message}");
            }
        } // Event handler for saving the updated schedule to the database

        private void DeleteSchedule_Click(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked and retrieve the schedule from its Tag property
            var button = sender as Button;
            var schedule = button?.Tag as ServiceSchedule;
            // If the schedule is null, show an error message and return
            if (schedule == null)
            {
                MessageBox.Show("Invalid schedule selection.");
                return;
            }
            // Show a confirmation dialog before deleting the schedule
            var result = MessageBox.Show(
                $"Are you sure you want to delete the schedule for {schedule.Service.Name} on {schedule.DayOfWeek}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            // If the user confirms, remove the schedule from the database and reload the schedules to reflect the change
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    db.ServiceSchedules.Remove(schedule);
                    db.SaveChanges();
                    MessageBox.Show("Schedule deleted successfully!");
                    LoadSchedules(); // Reload and apply current filter
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting schedule: {ex.Message}");
                }
            }
        }// Event handler for deleting a schedule from the database
        private void ClearInputs()
        {
            
            DayComboBox.SelectedItem = null;
            StartTimeBox.Text = "09:00";
            EndTimeBox.Text = "17:00";
            ActiveCheckBox.IsChecked = true;
        } // Method to clear the input fields after adding a schedule
    }
}
