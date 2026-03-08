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
    public partial class BusinessSignup : Window
    {
        public BusinessSignup()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BusinessLogin businessLogin = new BusinessLogin();
            businessLogin.Show();
            this.Close();
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            if (passwordPBX.Password != confirmPasswordPBX.Password)
            {
                MessageBox.Show("Passwords do not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (ServiceBookingContext db = new ServiceBookingContext())
            {
                var newBusiness = new Business
                {
                    Name = businessNameTBX.Text,
                    City = CityTBX.Text,
                    Email = emailTBX.Text,
                    PhoneNumber = phoneTBX.Text,
                    Password = passwordPBX.Password // TODO: Hash with BCrypt
                };

                db.Businesses.Add(newBusiness);
                db.SaveChanges();

                MessageBox.Show("Business account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                BusinessLogin businessLogin = new BusinessLogin();
                businessLogin.Show();
                this.Close();
            }
        }
    }
}