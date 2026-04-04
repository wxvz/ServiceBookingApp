using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Interaction logic for EditService.xaml
    /// </summary>
    public partial class EditServiceSchedulePage : Page
    {
        private ServiceBookingContext db = new ServiceBookingContext();
        private List<ServiceSchedule> allSchedules = new List<ServiceSchedule>();
        
        public EditServiceSchedulePage()
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
            if (SessionManager.CurrentBusiness == null) return;

            try
            {
                var services = db.Services
                    .Where(s => s.BusinessId == SessionManager.CurrentBusiness.BusinessId)
                    .ToList();

                ServiceFilterComboBox.ItemsSource = services;

                // Add a dummy Service object to act as the header/placeholder
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
            

            if (ServiceFilterComboBox == null || allSchedules == null) return;

            try
            {
                var selectedValue = ServiceFilterComboBox.SelectedValue;

                if (selectedValue == null || (int)selectedValue == -1)
                {
                    // Do not show any schedules until a specific service is selected
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
        }

        private void LoadDays()
        {
            var days = Enum.GetValues(typeof(DayOfWeek))
                .Cast<DayOfWeek>()
                .Select(d => d.ToString())
                .ToList();
            DayComboBox.ItemsSource = days;
        }

        private void SaveSchedule_Click(object sender, RoutedEventArgs e)
        {
            // If "Show All Schedules" is selected, we cannot add a schedule
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

                if (conflictingSchedule != null)
                {
                    MessageBox.Show("This schedule conflicts with an existing active schedule for the same service.");
                    return;
                }

                var newSchedule = new ServiceSchedule
                {
                    ServiceId = selectedServiceId,
                    DayOfWeek = selectedDay,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsActive = ActiveCheckBox.IsChecked ?? true
                };

                db.ServiceSchedules.Add(newSchedule);
                db.SaveChanges();

                MessageBox.Show("Schedule added successfully!");
                ClearInputs();
                LoadSchedules(); // Reload and apply current filter
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding schedule: {ex.Message}");
            }
        }

        private void EditSchedule_Click(object sender, RoutedEventArgs e)
        {
            // Get the button that was clicked
            var button = sender as Button;
            // Retrieve the schedule from the buttons Tag property
            var schedule = button?.Tag as ServiceSchedule;
            
            if (schedule == null)
            {
                MessageBox.Show("Invalid schedule selection.");
                return;
            }
            // Populate the input fields with the schedules data for editing
            ServiceFilterComboBox.SelectedValue = schedule.ServiceId;
            DayComboBox.SelectedItem = schedule.DayOfWeek.ToString();
            StartTimeBox.Text = schedule.StartTime.ToString(@"hh\:mm");
            EndTimeBox.Text = schedule.EndTime.ToString(@"hh\:mm");
            ActiveCheckBox.IsChecked = schedule.IsActive;

            ServiceFilterComboBox.Tag = schedule;

        }

        private void DeleteSchedule_Click(object sender, RoutedEventArgs e)
        {
  
            var button = sender as Button;
            var schedule = button?.Tag as ServiceSchedule;

            if (schedule == null)
            {
                MessageBox.Show("Invalid schedule selection.");
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete the schedule for {schedule.Service.Name} on {schedule.DayOfWeek}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

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
        }

        private void ClearInputs()
        {
            ServiceFilterComboBox.SelectedItem = null;
            ServiceFilterComboBox.Tag = null;
            DayComboBox.SelectedItem = null;
            StartTimeBox.Text = "09:00";
            EndTimeBox.Text = "17:00";
            ActiveCheckBox.IsChecked = true;
        }
    }
}
