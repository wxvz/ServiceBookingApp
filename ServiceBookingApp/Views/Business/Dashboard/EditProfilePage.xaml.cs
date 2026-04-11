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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServiceBookingApp
{
    /// <summary>
    /// Interaction logic for EditProfilePage.xaml
    /// </summary>
    public partial class EditProfilePage : Page
    {
        public ServiceBookingContext db = new ServiceBookingContext();
        public EditProfilePage()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBusinessDetails();
        }

        private void LoadBusinessDetails()
        {
            if (SessionManager.CurrentBusiness != null)
            {
                BusinessNameTBX.Text = SessionManager.CurrentBusiness.Name;
                AddressTBX.Text = SessionManager.CurrentBusiness.Address;
                PhoneTBX.Text = SessionManager.CurrentBusiness.PhoneNumber;
                EmailTBX.Text = SessionManager.CurrentBusiness.Email;
                if (!string.IsNullOrEmpty(SessionManager.CurrentBusiness.Description))
                {
                    DescriptionTBX.Text = SessionManager.CurrentBusiness.Description;
                }
                else
                {
                    DescriptionTBX.Text = "Enter in your business description.";
                }
            }
            else
            {
                this.NavigationService.Navigate(new BusinessLogin());
                MessageBox.Show("No business is currently logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var business = db.Businesses.Find(SessionManager.CurrentBusiness.BusinessId);
            if (business != null)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(BusinessNameTBX.Text) ||
                        string.IsNullOrWhiteSpace(AddressTBX.Text) ||
                        string.IsNullOrWhiteSpace(PhoneTBX.Text) ||
                        string.IsNullOrWhiteSpace(EmailTBX.Text))
                    {
                        MessageBox.Show("Please fill in all required fields (Name, Address, Phone, Email).");
                        return;
                    }
                    business.Name = BusinessNameTBX.Text.Trim();
                    business.Address = AddressTBX.Text.Trim();
                    business.PhoneNumber = PhoneTBX.Text.Trim();
                    business.Email = EmailTBX.Text.Trim();
                    business.Description = DescriptionTBX.Text.Trim();
                        
                    db.SaveChanges();
                    
                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
