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
    /// Interaction logic for CustomerSignup.xaml
    /// </summary>
    public partial class CustomerSignup : Page
    {
        public CustomerSignup()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            // Trim input fields to remove leading and trailing whitespace
            firstNameTBX.Text = firstNameTBX.Text.Trim();
            lastNameTBX.Text = lastNameTBX.Text.Trim();
            emailTBX.Text = emailTBX.Text.Trim();
            phoneTBX.Text = phoneTBX.Text.Trim();
            passwordPBX.Password = passwordPBX.Password.Trim();
            // Validating input fields
            if (passwordPBX.Password != confirmPasswordPBX.Password)
            {
                MessageBox.Show("Passwords do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                passwordPBX.Password = string.Empty;
                confirmPasswordPBX.Password = string.Empty;
                return;
            }
            // Check if any of the required fields are empty
            if (string.IsNullOrWhiteSpace(firstNameTBX.Text) ||
                string.IsNullOrWhiteSpace(lastNameTBX.Text) ||
                string.IsNullOrWhiteSpace(emailTBX.Text) ||
                string.IsNullOrWhiteSpace(phoneTBX.Text) ||
                string.IsNullOrWhiteSpace(passwordPBX.Password))
            {
                MessageBox.Show("Please fill in all fields!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // email must contain both @ and . to be valid
            if (emailTBX.Text.Contains('@') == false || emailTBX.Text.Contains('.') == false)
            {
                MessageBox.Show("Please enter a valid email address!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // phone number validation
            if (phoneTBX.Text.All(Char.IsDigit) == false || phoneTBX.Text.Length < 7)
            {
                MessageBox.Show("Please enter a valid phone number!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // db operations
            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                // Check if email already exists in the database
                var existingCustomer = db.Customers.FirstOrDefault(c => c.Email == emailTBX.Text);
                if (existingCustomer != null)
                {
                    MessageBox.Show("An account with this email already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                // If all validations pass, a new customer is saved to the database
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordPBX.Password); // Hash the password before saving to the database
                var newCustomer = new Customer
                {
                    Name = firstNameTBX.Text + " " + lastNameTBX.Text,
                    Email = emailTBX.Text,
                    PhoneNumber = phoneTBX.Text,
                    Password = hashedPassword 
                };
                db.Customers.Add(newCustomer);
                db.SaveChanges();
                MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.NavigationService.GoBack();
            } 
        } 
    }
}