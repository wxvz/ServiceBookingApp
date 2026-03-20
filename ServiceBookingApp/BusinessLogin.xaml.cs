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
    /// Interaction logic for BusinessLogin.xaml
    /// </summary>
    public partial class BusinessLogin : Page
    {
        ServiceBookingContext db = new ServiceBookingContext();

        public BusinessLogin()
        {
            InitializeComponent();
        }

        private void SignUpBusinessLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.Navigate(new BusinessSignup());
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailTextBox.Text) || string.IsNullOrWhiteSpace(passwordBox.Password))
                {
                    MessageBox.Show("Please enter both email and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string email = emailTextBox.Text;
                string password = passwordBox.Password;

                var business = db.Businesses.FirstOrDefault(b => b.Email == email);

                if (business == null)
                {
                    MessageBox.Show("No account found with that email.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    emailTextBox.Text = string.Empty;
                    return;
                }

                var businessEmail = db.Businesses
                    .Where(b => b.Email == email)
                    .Select(b => b.Email);

                var businessPassword = db.Businesses
                    .Where(b => b.Email == businessEmail.FirstOrDefault())
                    .Select(b => b.Password)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(businessPassword))
                {
                    MessageBox.Show("Incorrect password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    passwordBox.Password = string.Empty;
                    return;
                }

                // Verifying the password using BCrypt
                if (BCrypt.Net.BCrypt.Verify(password, businessPassword))
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    SessionManager.Login(business); // Set the current business in the session

                    this.NavigationService.Navigate(new BusinessDashboard());
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
