using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void customerBtn_Click(object sender, RoutedEventArgs e)
        {
            CustomerLogin cLogin = new CustomerLogin();
            cLogin.Show();
            this.Close();
        }

        private void businessBtn_Click(object sender, RoutedEventArgs e)
        {
            BusinessLogin bLogin = new BusinessLogin();
            bLogin.Show();
            this.Close();
        }
    }
}