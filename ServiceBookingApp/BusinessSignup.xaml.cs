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
            this.NavigationService.GoBack();
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (passwordPBX.Password != confirmPasswordPBX.Password)
            {
                MessageBox.Show("Passwords do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                passwordPBX.Password = string.Empty;   
                confirmPasswordPBX.Password = string.Empty;
                return;
            }

            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                // Hash the password before saving
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordPBX.Password);

                var newBusiness = new Business
                {
                    Name = businessNameTBX.Text,
                    City = CityTBX.Text,
                    Email = emailTBX.Text,
                    PhoneNumber = phoneTBX.Text,
                    Password = hashedPassword
                };

                db.Businesses.Add(newBusiness);
                db.SaveChanges();

                MessageBox.Show("Business account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                this.NavigationService.GoBack();
            }
        }
    }
}