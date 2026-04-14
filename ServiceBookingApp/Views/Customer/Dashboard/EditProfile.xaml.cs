using System;
using System.Collections.Generic;
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

namespace ServiceBookingApp.Views.Customer.Dashboard
{
    /// <summary>
    /// Interaction logic for EditProfile.xaml
    /// </summary>
    public partial class EditProfile : Page
    {
        private ServiceBookingContext db = new ServiceBookingContext();
        public EditProfile()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (SessionManager.CurrentCustomer == null)
            {
                this.NavigationService.Navigate(new CustomerLogin());
                MessageBox.Show("No customer is currently logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var customer = SessionManager.CurrentCustomer;
            NameTBX.Text = customer.Name;
            PhoneTBX.Text = customer.PhoneNumber;
            EmailTBX.Text = customer.Email;
        }


        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            var customer = SessionManager.CurrentCustomer;
            NameTBX.Text = customer.Name;
            PhoneTBX.Text = customer.PhoneNumber;
            EmailTBX.Text = customer.Email;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var customer = db.Customers.Find(SessionManager.CurrentCustomer.CustomerId);
            if (customer != null)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(NameTBX.Text) ||
                        string.IsNullOrWhiteSpace(PhoneTBX.Text) ||
                        string.IsNullOrWhiteSpace(EmailTBX.Text))
                    {
                        MessageBox.Show("Please fill in all required fields (Name, Phone, Email).");
                        return;
                    }
                    customer.Name = NameTBX.Text.Trim();
                    customer.PhoneNumber = PhoneTBX.Text.Trim();
                    customer.Email = EmailTBX.Text.Trim();


                    db.SaveChanges();

                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Window_Loaded(null, null); // Refresh the displayed details after saving
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
