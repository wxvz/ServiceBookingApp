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
    public partial class CustomerSignup : Window
    {
        public CustomerSignup()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            CustomerLogin customerLogin = new CustomerLogin();
            customerLogin.Show();
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
                var newCustomer = new Customer
                {
                    Name = firstNameTBX.Text + " " + lastNameTBX.Text,
                    Email = emailTBX.Text,
                    Address = string.Empty,
                    PhoneNumber = phoneTBX.Text,
                    Password = passwordPBX.Password // TODO: Hash with BCrypt
                };

                db.Customers.Add(newCustomer);
                db.SaveChanges();

                MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CustomerLogin customerLogin = new CustomerLogin();
                customerLogin.Show();
                this.Close();
            }
        }
    }
}