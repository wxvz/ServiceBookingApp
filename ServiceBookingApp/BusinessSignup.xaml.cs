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
            // Will implement the logic to sign up the business here later
        }
    }
}
