using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ServiceBookingApp.Data;
using ServiceBookingApp.Helper;

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for BusinessLogin.xaml
    /// </summary>
    public partial class BusinessLogin : Page
    {
        // Initialize the database context to interact with the database
        ServiceBookingContext db = new ServiceBookingContext();
        public BusinessLogin()
        {
            InitializeComponent();
        }
        
        private void SignUpBusinessLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Navigate to the BusinessSignUp page when the Sign Up link is clicked
            this.NavigationService.Navigate(new BusinessSignup());
        } // Event handler for the Sign Up link click

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            // Trim input to remove leading and trailing whitespace
                emailTextBox.Text = emailTextBox.Text.Trim();
                passwordBox.Password = passwordBox.Password.Trim();
            // Validate input and try to log in the business
            try
            {
                // Ensuring email and password are provided
                if (string.IsNullOrWhiteSpace(emailTextBox.Text) || string.IsNullOrWhiteSpace(passwordBox.Password))
                {
                    MessageBox.Show("Please enter both email and password.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                // Retrieve the email and password from the input fields
                string email = emailTextBox.Text;
                string password = passwordBox.Password;
                // Attempt to find the business with the provided email
                var business = db.Businesses.FirstOrDefault(b => b.Email == email);
                // If no business is found with that email, show an error message
                if (business == null)
                {
                    MessageBox.Show("No account found with that email.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Retrieve the stored email and password of the business via queries
                var businessEmail = db.Businesses
                    .Where(b => b.Email == email)
                    .Select(b => b.Email);
                var businessPassword = db.Businesses
                    .Where(b => b.Email == businessEmail.FirstOrDefault())
                    .Select(b => b.Password)
                    .FirstOrDefault();
                // If the password is null or empty, show an error message and return
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
                    SessionManager.LogIn(business); // Set the current business in the session

                    this.NavigationService.Navigate(new BusinessDashboard());
                }
                else // If the password is incorrect, show an error message and clear the password field
                {
                    MessageBox.Show("Incorrect password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    passwordBox.Password = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while trying to log in: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } // Event handler for the Login button click

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the HomePage when the Back button is clicked
            this.NavigationService.Navigate(new HomePage());
        } // Event handler for the Back button click
    }
}
