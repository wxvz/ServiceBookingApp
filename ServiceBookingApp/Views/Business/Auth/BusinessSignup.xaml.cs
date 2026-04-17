using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ServiceBookingApp.Models;
using ServiceBookingApp.Data;

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for BusinessSignup.xaml
    /// </summary>
    public partial class BusinessSignup : Page
    {
        public BusinessSignup()
        {
            InitializeComponent();
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the previous page (Business Login)
            this.NavigationService.GoBack();
        } // Event handler for the Back button click

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            // Trim input fields to remove leading and whitespace
            businessNameTBX.Text = businessNameTBX.Text.Trim();
            AddressTBX.Text = AddressTBX.Text.Trim();
            emailTBX.Text = emailTBX.Text.Trim();
            phoneTBX.Text = phoneTBX.Text.Trim();
            passwordPBX.Password = passwordPBX.Password.Trim();
            // Validate that passwords match
            if (passwordPBX.Password != confirmPasswordPBX.Password)
            {
                MessageBox.Show("Passwords do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                passwordPBX.Password = string.Empty;   
                confirmPasswordPBX.Password = string.Empty;
                return;
            }
            // Basic email format validation
            if (emailTBX.Text.Contains("@") == false || emailTBX.Text.Contains(".") == false)
            {
                MessageBox.Show("Please enter a valid email address!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Basic phone number validation (digits only and at least 7 characters)
            if (phoneTBX.Text.All(char.IsDigit) == false || phoneTBX.Text.Length < 7)
            {
                MessageBox.Show("Please enter a valid phone number!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Check if any field is empty
            if (string.IsNullOrWhiteSpace(businessNameTBX.Text) ||
                string.IsNullOrWhiteSpace(AddressTBX.Text) ||
                string.IsNullOrWhiteSpace(emailTBX.Text) ||
                string.IsNullOrWhiteSpace(phoneTBX.Text) ||
                string.IsNullOrWhiteSpace(passwordPBX.Password))
            {
                MessageBox.Show("Please fill in all fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // db operations
            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                // Checking if a business with the same email already exists
                var existingBusiness = db.Businesses.FirstOrDefault(b => b.Email == emailTBX.Text);
                if (existingBusiness != null)
                {
                    MessageBox.Show("An account with this email already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // Since business does not exist Proceed
                // Hash the password before saving
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordPBX.Password);
                // Create a new business object and save it to the database
                var newBusiness = new Business
                {
                    Name = businessNameTBX.Text,
                    Address = AddressTBX.Text,
                    Email = emailTBX.Text,
                    PhoneNumber = phoneTBX.Text,
                    Password = hashedPassword
                };
                // Add the new business to the database and save changes
                db.Businesses.Add(newBusiness);
                db.SaveChanges();
                MessageBox.Show("Business account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // Navigate to Previous Page (Business Login)
                this.NavigationService.GoBack();
            }
        } // Event handler for the Sign Up button click
    }
}