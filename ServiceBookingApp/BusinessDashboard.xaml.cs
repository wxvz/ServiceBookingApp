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
    /// Interaction logic for BusinessDashboard.xaml
    /// </summary>
    public partial class BusinessDashboard : Page
    {
        ServiceBookingContext db = new ServiceBookingContext();
        public BusinessDashboard()
        {
            InitializeComponent();
        }

        private void LoadBusiness(object sender, RoutedEventArgs e)
        {
            SessionManager.LoadSession();
            businessName.Text = SessionManager.CurrentBusiness.Name;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
