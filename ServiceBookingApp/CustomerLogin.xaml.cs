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
using System.Windows.Shapes;

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for CustomerLogin.xaml
    /// </summary>
    public partial class CustomerLogin : Page
    {
        ServiceBookingContext db = new ServiceBookingContext();
        public CustomerLogin()
        {
            InitializeComponent();
        }

        private void SignUpCustomerLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new CustomerSignup());
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(emailTextBox.Text) || string.IsNullOrWhiteSpace(passwordBox.Password)) //cHECK FOR WHITESPACE OR NULL
                {
                    MessageBox.Show("Please enter both email and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string email = emailTextBox.Text;
                string password = passwordBox.Password;

                var customer = db.Customers.FirstOrDefault(c => c.Email == email);

                if (customer == null)
                {
                    MessageBox.Show("No account found with that email.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    emailTextBox.Text = string.Empty;
                    return;
                }

                var customerEmail = db.Customers
                    .Where(c => c.Email == email)
                    .Select(c => c.Email);

                var customerPassword = db.Customers
                    .Where(c => c.Email == customerEmail.FirstOrDefault())
                    .Select(c => c.Password)
                    .FirstOrDefault();

                if (!customerPassword.Any()) // Check if password exists for the email
                {
                    MessageBox.Show("Incorrect password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    passwordBox.Password = string.Empty;
                    return;
                }

                // Verifying the password using BCrypt
                if (BCrypt.Net.BCrypt.Verify(password, customerPassword)) // Compare the entered password with the hashed password in the database
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    SessionManager.Login(customer); // Set the current customer in the session

                    this.NavigationService.Navigate(new CustomerDashboard());
                }
                else
                {
                    MessageBox.Show("Incorrect password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    passwordBox.Password = string.Empty;
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to log in: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
